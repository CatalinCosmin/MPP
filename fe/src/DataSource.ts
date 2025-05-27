import axios from "axios";
import { ConnectivityService } from "./services/ConnectivityService";
import { OfflineQueue } from "./services/OfflineQueue";

const authHeaders = () => ({
  headers: {
    Authorization: `Bearer ${localStorage.getItem('jwt')}`
  }
});

const apiUrl = process.env.REACT_APP_API_URL;


export type Car = {
  id: string;
  key: string;
  name: string;
  brand: string;
  model: string;
  year: number;
  color: string;
  price: number;
};

export class DataSource {
  private items: Car[] = [];
  private offset = 0;
  private limit = 10;

  constructor() {
    this.offset = 0;
    this.limit = 10;
  }

  private saveToLocal() {
    localStorage.setItem("cars", JSON.stringify(this.items));
  }

  private loadFromLocal(): Car[] {
    const stored = localStorage.getItem("cars");
    return stored ? JSON.parse(stored) : [];
  }

  async init() {
    const { network, server } = await ConnectivityService.isAvailable();

    if (!network || !server) {
      this.items = this.loadFromLocal();
      return;
    }

    try {
      const result = await axios.get<Car[]>(
        `${apiUrl}/Cars?Offset=${this.offset}&Limit=${this.limit}`,
        authHeaders()
      );

      this.items = result.data.map((car: Car) => {
        return { ...car, key: car.name + car.id };
      });
      this.saveToLocal();
    } catch (error) {
      console.error("Failed to load cars from API", error);
    }
  }
  async add(item: Car) {
    item.key = item.name + item.id;
    const { network, server } = await ConnectivityService.isAvailable();

    if (!network || !server) {
      this.items.push(item);
      this.saveToLocal();
      OfflineQueue.add({ type: "add", payload: item });
      return;
    }

    try {
      const response = await axios.post<Car>(
        `${apiUrl}/Cars`,
        item,
        authHeaders()
      );

      const createdCar = response.data;

      createdCar.key = createdCar.name + createdCar.id;
      this.items.push(createdCar);
      this.saveToLocal();
    } catch (error) {
      console.error("Failed to add car", error);
    }
  }

  async update(item: Car) {
    const { network, server } = await ConnectivityService.isAvailable();

    if (!network || !server) {
      const index = this.items.findIndex((car) => car.id === item.id);
      if (index !== -1) {
        this.items[index] = { ...item };
      }
      this.saveToLocal();
      OfflineQueue.add({ type: "update", payload: item });
      return;
    }

    try {
      await axios.patch(
        `${apiUrl}/Cars/${item.id}`,
        item,
        authHeaders()
      );

      const index = this.items.findIndex((car) => car.id === item.id);
      if (index !== -1) {
        this.items[index] = { ...item };
        this.saveToLocal();
      }
    } catch (error) {
      console.error("Failed to update car", error);
    }
  }

  async delete(name: string) {
    const { network, server } = await ConnectivityService.isAvailable();

    if (!network || !server) {
      this.items = this.items.filter((item) => item.name !== name);
      this.saveToLocal();
      OfflineQueue.add({ type: "delete", payload: name });
      return;
    }

    try {
      await axios.delete(
        `${apiUrl}/Cars/${name}`,
        authHeaders()
      );

      this.items = this.items.filter((item) => item.name !== name);
      this.saveToLocal();
    } catch (error) {
      console.error("Failed to delete car", error);
    }
  }


  async getAll(filterName?: string, offset: number = 0, limit: number = 10): Promise<Car[]> {
    const { network, server } = await ConnectivityService.isAvailable();
    if (!network || !server) {
      const localCars = this.loadFromLocal();
      const filtered = filterName
        ? localCars.filter((car) => car.name.toLowerCase().includes(filterName.toLowerCase()))
        : localCars;
      return filtered.slice(offset, offset + limit);
    }

    console.log("getting data from backend", this.items)

    try {
      const response = await axios.get(`${apiUrl}/Cars`, {
        ...authHeaders(),
        params: {
          Offset: offset,
          Limit: limit,
          FilterName: filterName || undefined
        }
      });


      this.items = response.data.result;
      this.saveToLocal();
      return [...this.items];
    } catch (error) {
      console.error("Failed to fetch cars", error);
      return [];
    }
  }

  clear() {
    this.items = [];
    localStorage.removeItem("cars");
  }

  async syncOfflineChanges() {
    const queued = OfflineQueue.get();
    console.log("syncing")
    console.log(queued)
    for (const op of queued) {
      try {
        if (op.type === "add") {
          await axios.post(`${apiUrl}/Cars`, op.payload, authHeaders());
        } else if (op.type === "update") {
          await axios.patch(`${apiUrl}/Cars/${op.payload.id}`, op.payload, authHeaders());
        } else if (op.type === "delete") {
          await axios.delete(`${apiUrl}/Cars/${op.payload}`, authHeaders());
        }
      } catch (e) {
        console.error("Failed to sync queued op", op, e);
        return;
      }
    }
    this.getAll();

    OfflineQueue.clear();
  }
}

export const CarRepository = new DataSource();
