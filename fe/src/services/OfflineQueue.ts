export type QueuedOperation = {
    type: "add" | "update" | "delete";
    payload: any;
  };
  
export class OfflineQueue {
    private static key = "offlineCarOps";
  
    static get(): QueuedOperation[] {
      const stored = localStorage.getItem(this.key);
      return stored ? JSON.parse(stored) : [];
    }
  
    static add(op: QueuedOperation) {
      const ops = this.get();
      ops.push(op);
      localStorage.setItem(this.key, JSON.stringify(ops));
      console.log(ops)
    }
  
    static clear() {
      localStorage.removeItem(this.key);
    }
  }
  