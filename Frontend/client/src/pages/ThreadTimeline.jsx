import { useEffect } from "react";
import { startConnection, onThreadUpdate } from "../services/signalrService";

const threads = [1, 2, 3, 4, 5];

const ThreadTimeline = () => {
  useEffect(() => {
    startConnection();

    onThreadUpdate((data) => {
      console.log("Thread Data:", data);
    });
  }, []);

  return (
    <div className="h-screen bg-gray-900 text-white p-4">
      <h1 className="text-2xl font-bold mb-4">Thread Timeline Visualization</h1>

      <div className="border border-gray-700 h-full rounded-lg flex gap-6 p-4 overflow-x-auto">
        {threads.map((threadId) => (
          <div key={threadId} className="flex flex-col items-center">
            {/* Thread Label */}
            <div className="mb-2 text-sm text-gray-400">Thread {threadId}</div>

            {/* Vertical Line */}
            <div className="w-1 bg-green-500 h-full"></div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default ThreadTimeline;
