import axios from "axios";

const apiUrl = process.env.REACT_APP_API_URL;

export class ConnectivityService {
    static async isOnline(): Promise<boolean> {
      return navigator.onLine;
    }
  
    static async isServerAvailable(): Promise<boolean> {
      try {
        await axios.get(`${apiUrl}/health`);
        return true;
      } catch {
        return false;
      }
    }
  
    static async isAvailable(): Promise<{ network: boolean; server: boolean }> {
      const [network, server] = await Promise.all([
        this.isOnline(),
        this.isServerAvailable(),
      ]);
      return { network, server };
    }
  }
  