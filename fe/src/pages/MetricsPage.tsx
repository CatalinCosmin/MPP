import { Card } from "antd";
import { useEffect, useState } from "react";
import { LineChart, Line, XAxis, YAxis, Tooltip, ResponsiveContainer } from "recharts";
import "./MetricsPage.css";
import * as signalR from "@microsoft/signalr";
import axios from "axios";

const authHeaders = () => ({
  headers: {
    Authorization: `Bearer ${localStorage.getItem('jwt')}`
  }
});


export const MetricsPage = () => {
    const [data, setData] = useState<{ time: string; total: number; sum: number; avg: number }[]>([]);
    const [metrics, setMetrics] = useState<Metrics | null>(null);

    const updateMetrics = async () => {
        try {
            const response = await axios.get<Metrics>(process.env.REACT_APP_API_URL + "/metrics", authHeaders());
            if (response.status === 200) {
                const newMetrics = response.data;

                // Retrieve the existing metrics array from localStorage, or initialize an empty array
                const storedMetrics = JSON.parse(localStorage.getItem("metrics") || "[]");

                // Append the new metrics to the array
                storedMetrics.push(newMetrics);

                console.log("saving metrics", storedMetrics)

                // Store the updated array back to localStorage
                localStorage.setItem("metrics", JSON.stringify(storedMetrics));

                // Update the metrics state
                setMetrics(newMetrics);

                const total = newMetrics.totalCars;
                const sum = newMetrics.totalPrice;
                const avg = newMetrics.averagePrice;

                // Add a new data point for the chart
                setData((prevData) => [
                    ...prevData.slice(-500), // Limit to last 500 entries
                    {
                        time: `${new Date().getHours()}:${new Date().getMinutes()}`,
                        total,
                        sum,
                        avg,
                    },
                ]);

                console.log(newMetrics, total, sum, avg);
            } else {
                console.error("Error fetching metrics");
            }
        } catch (error) {
            console.error("Error in updateMetrics:", error);
        }
    };

    // Load metrics from localStorage on component mount
    useEffect(() => {
        const storedMetrics = localStorage.getItem("metrics");
        if (storedMetrics !== null) {
            const parsedMetrics: Metrics[] = JSON.parse(storedMetrics); // Parse as array of metrics
            console.log(parsedMetrics, "stored metrics")
            setMetrics(parsedMetrics[parsedMetrics.length - 1]); // Set the most recent metrics for display
            console.log("Loaded metrics from localStorage", parsedMetrics);
            // Update chart data from the stored metrics
            const chartData = parsedMetrics.map((metric, index) => ({
                time: `${new Date().getHours()}:${new Date().getMinutes()}`,
                total: metric.totalCars,
                sum: metric.totalPrice,
                avg: metric.averagePrice,
            }));
            setData(chartData);
        } else {
            console.log("No metrics in localStorage, fetching from server...");
            updateMetrics(); // Fetch data from the server if not in localStorage
        }

        const connection = new signalR.HubConnectionBuilder()
            .withUrl(process.env.REACT_APP_API_URL+"/entityHub", {
                skipNegotiation: true,
                transport: signalR.HttpTransportType.WebSockets,
            })
            .withAutomaticReconnect()
            .build();

        connection.start()
            .then(() => console.log("MetricsPage SignalR connected"))
            .catch((err) => console.error("MetricsPage SignalR error: ", err));

        connection.serverTimeoutInMilliseconds = 60000;

        connection.on("MetricsChanged", () => {
            console.log("MetricsPage: MetricsChanged event received");
            updateMetrics(); // Update metrics when the event is received
        });

        return () => {
            connection.stop();
        };
    }, []); // Empty dependency array to run only once on mount

    return (
        <div>
            <div className="metrics-container">
                <Card title="Total Cars" className="metric-card">
                    <ResponsiveContainer width="100%" height={150}>
                        <LineChart data={data}>
                            <XAxis dataKey="time" />
                            <YAxis />
                            <Tooltip />
                            <Line type="monotone" dataKey="total" stroke="#8884d8" strokeWidth={2} />
                        </LineChart>
                    </ResponsiveContainer>
                </Card>

                <Card title="Total Car Price" className="metric-card">
                    <ResponsiveContainer width="100%" height={150}>
                        <LineChart data={data}>
                            <XAxis dataKey="time" />
                            <YAxis />
                            <Tooltip />
                            <Line type="monotone" dataKey="sum" stroke="#82ca9d" strokeWidth={2} />
                        </LineChart>
                    </ResponsiveContainer>
                </Card>
            </div>

            <div className="metrics-container">
                <Card title="Average Car Price" className="metric-card">
                    <ResponsiveContainer width="100%" height={150}>
                        <LineChart data={data}>
                            <XAxis dataKey="time" />
                            <YAxis />
                            <Tooltip />
                            <Line type="monotone" dataKey="avg" stroke="#ff7300" strokeWidth={2} />
                        </LineChart>
                    </ResponsiveContainer>
                </Card>
            </div>
        </div>
    );
};

type Metrics = {
    totalPrice: number;
    totalCars: number;
    averagePrice: number;
}
