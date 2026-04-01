import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
  .withUrl("https://localhost:5001/threadHub")
  .withAutomaticReconnect()
  .build();

export const startConnection = async () => {
  try {
    await connection.start();
    console.log("Connected to SignalR");
  } catch (err) {
    console.error("Connection failed:", err);
    setTimeout(startConnection, 5000);
  }
};

export const onThreadUpdate = (callback) => {
  connection.on("ReceiveThreadUpdate", callback);
};
