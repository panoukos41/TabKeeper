using Core;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Buffers;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace TabKeeper.Components;

public sealed class FluentValidator<TModel> : ComponentBase, IDisposable
{
    private (EditContext? editContext, IValidator<TModel>? validator) previus = (null, null);
    private readonly CompositeDisposable disposables = [];
    private readonly CompositeDisposable notificationSubs = [];

    [CascadingParameter]
    public EditContext EditContext { get; set; } = null!;

    [Parameter]
    public IValidator<TModel>? Validator { get; set; }

    [Parameter]
    public IObservable<ValidationResult>? ResultNotifications { get; set; }

    [Parameter]
    public IObservable<List<ValidationFailure>>? FailureNotifications { get; set; }

    public ValidationMessageStore MessageStore { get; private set; } = null!;

    protected override void OnParametersSet()
    {
        ArgumentNullException.ThrowIfNull(EditContext, nameof(EditContext));

        notificationSubs.Clear();
        if (ResultNotifications is { })
        {
            ResultNotifications.Subscribe(SetValidationResult).DisposeWith(notificationSubs);
        }
        if (FailureNotifications is { })
        {
            FailureNotifications.Select(failures => new ValidationResult(failures)).Subscribe(SetValidationResult).DisposeWith(notificationSubs);
        }

        if (EditContext != previus.editContext || Validator != previus.validator)
            Setup();
    }

    private void Setup()
    {
        disposables.Clear();
        previus = (EditContext, Validator);
        MessageStore = new(EditContext);

        Observable.FromEventPattern<ValidationRequestedEventArgs>(
            h => EditContext.OnValidationRequested += h,
            h => EditContext.OnValidationRequested -= h)
            .Subscribe(_ => ValidationRequested())
            .DisposeWith(disposables);

        Observable.FromEventPattern<FieldChangedEventArgs>(
            h => EditContext.OnFieldChanged += h,
            h => EditContext.OnFieldChanged -= h)
            .Subscribe(_ => FieldChanged(_.EventArgs.FieldIdentifier))
            .DisposeWith(disposables);
    }

    private async void ValidationRequested()
    {
        MessageStore.Clear();
        if (Validator is null) return;

        var validationContext = new ValidationContext<object>(EditContext.Model);
        var result = await Validator.ValidateAsync(validationContext);

        AddValidationResult(EditContext.Model, result);
    }

    async void FieldChanged(FieldIdentifier fieldIdentifier)
    {
        MessageStore.Clear(fieldIdentifier);

        if (Validator is null) return;

        var propertiesToValidate = new string[] { fieldIdentifier.FieldName };
        var fluentValidationContext = new ValidationContext<object>(
            instanceToValidate: fieldIdentifier.Model,
            propertyChain: new FluentValidation.Internal.PropertyChain(),
            validatorSelector: new FluentValidation.Internal.MemberNameValidatorSelector(propertiesToValidate)
        );

        var result = await Validator.ValidateAsync(fluentValidationContext);

        AddValidationResult(fieldIdentifier.Model, result);
    }

    private void AddValidationResult(object model, ValidationResult validationResult)
    {
        foreach (ValidationFailure error in validationResult.Errors)
        {
            var fieldIdentifier = ToFieldIdentifier(model, error.PropertyName);
            MessageStore.Add(fieldIdentifier, error.ErrorMessage);
        }
        EditContext.NotifyValidationStateChanged();
    }

    public void AddValidationResult(ValidationResult validationResult)
    {
        AddValidationResult(EditContext.Model, validationResult);
    }

    public void SetValidationResult(ValidationResult validationResult)
    {
        MessageStore.Clear();
        AddValidationResult(EditContext.Model, validationResult);
    }

    private static readonly SearchValues<char> separators = SearchValues.Create(".[");

    private static FieldIdentifier ToFieldIdentifier(object model, string propertyPath)
    {
        // Code modified from: https://blog.stevensanderson.com/2019/09/04/blazor-fluentvalidation/
        // This method parses property paths like 'SomeProp.MyCollection[123].ChildProp'
        // and returns a FieldIdentifier which is an (instance, propName) pair. For example,
        // it would return the pair (SomeProp.MyCollection[123], "ChildProp"). It traverses
        // as far into the propertyPath as it can go until it finds any null instance.

        var span = propertyPath.AsSpan();
        var obj = model;
        var firstRun = true;

        while (true)
        {
            var nextTokenEnd = span.IndexOfAny(separators);
            if (nextTokenEnd < 0)
            {
                return new FieldIdentifier(obj, firstRun ? propertyPath : span.ToString());
            }
            firstRun = false;

            var nextToken = span[..nextTokenEnd];
            span = span[(nextTokenEnd + 1)..];

            object? newObj;
            if (nextToken.EndsWith("]"))
            {
                // It's an indexer
                // This code assumes C# conventions (one indexer named Item with one param)
                nextToken = nextToken[..^1];
                var prop = obj.GetType().GetProperty("Item")!;
                var indexerType = prop.GetIndexParameters()[0].ParameterType;
                var indexerValue = Convert.ChangeType(nextToken.ToString(), indexerType);
                newObj = prop!.GetValue(obj, [indexerValue]);
            }
            else
            {
                // It's a regular property
                var prop = obj.GetType().GetProperty(nextToken.ToString())
                    ?? throw new InvalidOperationException($"Could not find property named {nextToken} on object of type {obj.GetType().FullName}.");
                newObj = prop.GetValue(obj);
            }
            if (newObj == null)
            {
                // This is as far as we can go
                return new FieldIdentifier(obj, nextToken.ToString());
            }
            obj = newObj;
        }
    }

    public void Dispose()
    {
        disposables.Dispose();
        notificationSubs.Dispose();
    }
}
