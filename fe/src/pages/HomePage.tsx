import { Table, Input } from "antd";
import { useCallback, useEffect, useRef, useState } from "react";
import { Car } from "../DataSource";
import { CarRepository } from "../DataSource";
import "./HomePage.css";
import * as signalR from "@microsoft/signalr";

export const HomePage = () => {
    const [cars, setCars] = useState<Car[]>([]);
    const [searchQuery, setSearchQuery] = useState("");
    const [pageSize] = useState(20);
    const [loading, setLoading] = useState(false);

    const scrollRef = useRef<HTMLDivElement>(null);

    const [mostExpensivePrice, setMostExpensivePrice] = useState<number | null>(null);
    const [leastExpensivePrice, setLeastExpensivePrice] = useState<number | null>(null);

    const offsetRef = useRef(0);

    const fetchMoreCars =  useCallback(async () => {
        if (loading) return;

        setLoading(true);
        const currentOffset = offsetRef.current;

        const newCars = await CarRepository.getAll(searchQuery, currentOffset, pageSize);
        console.log("retrieved " + newCars.length + " more cars")
        offsetRef.current += newCars.length;

        if(newCars.length !== 0)
            setCars(prev => {
                const ids = new Set(prev.map(car => car.id));
                const uniqueNewCars = newCars.filter(car => !ids.has(car.id));
                const combined = [...prev, ...uniqueNewCars];

                const prices = combined.map(c => c.price);
                setMostExpensivePrice(Math.max(...prices));
                setLeastExpensivePrice(Math.min(...prices));

                return combined;
            });

        setLoading(false);
    }, [searchQuery, loading]);

    useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:7092/entityHub")
            .withAutomaticReconnect()
            .build();

        connection.start()
            .then(() => {
                console.log("Connected to SignalR hub");
            })
            .catch(err => console.error("SignalR Connection Error: ", err));

        connection.on("CarsChanged", () => {
            console.log("CarsChanged event triggered")
            console.log(cars.length)
            if (cars.length === 0 || cars.length <= pageSize)
                fetchMoreCars();
            else
                handleScroll();
        });

        return () => {
            connection.stop();
        };
    }, []);


    const handleScroll = () => {
        const container = scrollRef.current;
        if (!container) return;

        const { scrollTop, scrollHeight, clientHeight } = container;
        const nearBottom = scrollHeight - scrollTop <= clientHeight + 50;

        console.log("scroll handle: ", nearBottom, loading)
        if (nearBottom && !loading) {
            fetchMoreCars();
        }
    };


    useEffect(() => {
        const container = scrollRef.current;
        if (container) {
            container.addEventListener("scroll", handleScroll);
        }

        return () => {
            if (container) {
                container.removeEventListener("scroll", handleScroll);
            }
        };
    }, [loading, offsetRef, searchQuery]);

    useEffect(() => {
        fetchMoreCars();
        offsetRef.current = 0;
        setMostExpensivePrice(null);
        setLeastExpensivePrice(null);
    }, [searchQuery]);

    const handleSearch = (value: string) => {
        setSearchQuery(value);
    };

    const columns = [
        {
            title: "Name",
            dataIndex: "name",
            key: "name",
            filterDropdown: ({ setSelectedKeys, selectedKeys, confirm }: any) => (
                <div style={{ padding: 8 }}>
                    <Input
                        value={selectedKeys[0]}
                        onChange={e => setSelectedKeys(e.target.value ? [e.target.value] : [])}
                        onPressEnter={() => {
                            handleSearch(selectedKeys[0]);
                            confirm();
                        }}
                        placeholder="Search Name"
                        style={{ marginBottom: 8, display: "block" }}
                    />
                </div>
            )
        },
        { title: "Brand", dataIndex: "brand", key: "brand" },
        { title: "Model", dataIndex: "model", key: "model" },
        { title: "Year", dataIndex: "year", key: "year" },
        { title: "Color", dataIndex: "color", key: "color" },
        { title: "Price", dataIndex: "price", key: "price" }
    ];

    return (
        <div className="body" ref={scrollRef} style={{ height: "80vh", overflowY: "auto" }}>
            <div className="dataTableWrapper">
                <Table
                    className="dataTable"
                    dataSource={cars}
                    columns={columns}
                    pagination={false}
                    rowClassName={(record) => {
                        if (record.price === mostExpensivePrice) return "most-expensive";
                        if (record.price === leastExpensivePrice) return "least-expensive";
                        return "";
                    }}
                />
                {loading && <div style={{ textAlign: "center", padding: 16 }}>Loading...</div>}
            </div>
        </div>
    );
};