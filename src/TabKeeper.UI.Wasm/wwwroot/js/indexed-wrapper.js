class DatabaseDefinition {
  /** @type {string} */
  name;

  /** @type {number} */
  version;

  /** @type {ObjectStoreDefinition[]} */
  objectStores;

  /** @param {DatabaseDefinition} definition */
  constructor(definition) {
    this.name = definition.name;
    this.version = definition.version;
    this.objectStores = definition.objectStores.map(s => new ObjectStoreDefinition(s));
  }

  /** 
   * @param {IDBDatabase} db
   * @param {IDBTransaction} transaction
   */
  apply(db, transaction) {
    const objectStoreNames = [...db.objectStoreNames];

    // delete stores
    objectStoreNames
      .filter(objectStore => this.objectStores.find(x => x.name == objectStore) == null)
      .forEach(objectStore => db.deleteObjectStore(objectStore));

    // edit existing stores
    this.objectStores
      .filter(objectStore => objectStoreNames.find(x => x == objectStore.name) != null)
      .forEach(objectStore => objectStore.update(transaction));

    // add new stores with indexes.
    this.objectStores
      .filter(objectStore => objectStoreNames.find(x => x == objectStore.name) == null)
      .forEach(objectStore => objectStore.create(db));
  }
}

class IndexDefinition {
  /** @type {string} */
  name;

  /** @type {string} */
  keyPath;

  /** @type {?boolean} */
  multiEntry;

  /** @type {?boolean} */
  unique;

  /** @param {IndexDefinition} definition */
  constructor(definition) {
    this.name = definition.name;
    this.keyPath = definition.keyPath;
    this.multiEntry = definition.multiEntry;
    this.unique = definition.unique;
  }

  /** @param {IDBObjectStore} store */
  create(store) {
    store.createIndex(this.name, this.keyPath, { multiEntry: this.multiEntry, unique: this.unique })
  }
}

class ObjectStoreDefinition {
  /** @type {string} */
  name;

  /** @type {string} */
  keyPath;

  /** @type {?boolean} */
  autoIncrement;

  /** @type {IndexDefinition[]} */
  indexes;

  /** @param {ObjectStoreDefinition} definition */
  constructor(definition) {
    this.name = definition.name;
    this.keyPath = definition.keyPath;
    this.autoIncrement = definition.autoIncrement;
    this.indexes = definition.indexes.map(i => new IndexDefinition(i));
  }

  /** @param {IDBDatabase} db */
  create(db) {
    const store = db.createObjectStore(this.name, { keyPath: this.keyPath, autoIncrement: this.autoIncrement });
    this.indexes.forEach(index => index.create(store));
  }

  /** @param {IDBTransaction} transaction */
  update(transaction) {
    const store = transaction.objectStore(this.name);
    const indexNames = [...store.indexNames];

    // Delete indexes
    indexNames
      .filter(index => !this.containsIndex(index))
      .forEach(index => store.deleteIndex(index));

    // Create indexes
    this.indexes
      .filter(index => indexNames.find(x => x == index.name) == null)
      .forEach(index => index.create(store));
  }

  /**
   * @param {any} index
   * @returns {boolean}
   */
  containsIndex(index) {
    return this.indexes?.find(x => x.name == index) != null;
  }
}

/**
 * @param {DatabaseDefinition} definition
 * @returns {Promise}
 */
export function open(definition) {
  definition = new DatabaseDefinition(definition);

  return new Promise((resolve, reject) => {
    const request = indexedDB.open(definition.name, definition.version);

    request.onupgradeneeded = (args) => {
      const db = request.result;
      const transaction = request.transaction;
      definition.apply(db, transaction);
    }
    request.onblocked = (args) => {
    }
    request.onsuccess = (args) => {
      const db = new Database(request.result);
      resolve(db);
    };
    request.onerror = (args) => {
      reject(request.error);
    }
  });
}

export function databases() {
  return indexedDB.databases();
}

/**
 * @param {string} name
 * @returns {boolean}
 */
export function deleteDatabase(name) {
  return new Promise((resolve, reject) => {
    const request = indexedDB.deleteDatabase(name);
    request.onsuccess = (args) => {
      resolve(request.result == null);
    };
    request.onerror = (args) => {
      reject(request.error);
    };
  });
}

class Database {
  /** @param db {IDBDatabase} */
  constructor(db) {
    this.db = db;
  }

  getInfo() {
    return {
      name: this.db.name,
      version: this.db.version,
      objectStoreNames: [... this.db.objectStoreNames],
    }
  };

  /**
   * @param name {string}
   * @returns {ObjectStore}
   */
  objectStore(name) {
    return new ObjectStore(this.db, name);
  }

  close() {
    this.db.close();
  }
}

class ObjectStore {
  /** 
   * @param db {IDBDatabase}
   * @param store {string}
   */
  constructor(db, store) {
    this.db = db;
    this.store = store;
  }

  /** @returns {IDBObjectStore} */
  get objectStore() {
    const t = this.db.transaction(this.store, "readwrite");
    return t.objectStore(this.store);
  }

  /**
   * @template {T}
   * @param {IDBRequest<T>} request
   * @param {(request:IDBRequest<T>, args:any) => T} onsuccess
   * @returns {Promise<T>}
   */
  executeRequest(request, onsuccess = null) {
    return new Promise((resolve, reject) => {
      request.onsuccess = (args) => {
        onsuccess != null
          ? resolve(onsuccess(request, args))
          : resolve(request.result);
      }
      request.onerror = (args) => {
        reject(request.error);
      }
    });
  }

  getInfo() {
    const objectStore = this.objectStore;
    return {
      name: objectStore.name,
      keyPath: objectStore.keyPath,
      autoIncrement: objectStore.autoIncrement,
      indexNames: [... objectStore.indexNames],
    }
  }

  count() {
    return this.executeRequest(this.objectStore.count());
  }

  // todo: implement query
  get(key) {
    return this.executeRequest(this.objectStore.get(key))
  }

  // todo: implement query
  //getKey() {
  //}

  // todo: implement query
  getAll() {
    return this.executeRequest(this.objectStore.getAll());
  }

  // todo: implement query
  getAllKeys() {
    return this.executeRequest(this.objectStore.getAllKeys());
  }

  add(obj) {
    return this.executeRequest(this.objectStore.add(obj));
  }

  put(obj) {
    return this.executeRequest(this.objectStore.put(obj));
  }

  delete(key) {
    return this.executeRequest(this.objectStore.delete(key));
  }

  clear() {
    return this.executeRequest(this.objectStore.clear());
  }

  //openCursor() {
  //}

  //openKeyCursor() {
  //}
}
