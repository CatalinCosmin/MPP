import { ConnectivityService } from "./ConnectivityService";

export const ConnectionState = {
    isOffline: false,
    isServerDown: false,
  };
  
  setInterval(async () => {
    const status = await ConnectivityService.isAvailable();
    ConnectionState.isOffline = !status.network;
    ConnectionState.isServerDown = status.network && !status.server;
  }, 5000);
  