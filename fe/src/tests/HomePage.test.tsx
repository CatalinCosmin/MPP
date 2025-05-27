import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { HomePage } from '../pages/HomePage';
import { Car, CarRepository } from '../DataSource';

// Mock the CarRepository
jest.mock('../DataSource', () => ({
  CarRepository: {
    getAll: jest.fn()
  }
}));

const allCars: Car[] = [
  { id: "1", key: "car1", name: 'Tesla', brand: 'Tesl', model: 'Model S', year: 2022, color: 'Red', price: 100000 },
  { id: "2", key: "car2", name: 'BMW', brand: 'BMv', model: 'X5', year: 2020, color: 'Black', price: 80000 },
  { id: "3", key: "car3", name: 'Audi', brand: 'Aud', model: 'A6', year: 2021, color: 'White', price: 70000 }
];

beforeEach(() => {
  jest.clearAllMocks();
  // Default mock returns all cars
  (CarRepository.getAll as jest.Mock).mockResolvedValue(allCars);
});

test('renders the table with car data', async () => {
  render(<HomePage />);
  await waitFor(() => {
    expect(screen.getByText('Tesla')).toBeInTheDocument();
    expect(screen.getByText('BMW')).toBeInTheDocument();
    expect(screen.getByText('Audi')).toBeInTheDocument();
  });
});

test('renders correct number of rows', async () => {
  render(<HomePage />);
  await waitFor(() => {
    const rows = screen.getAllByRole('row');
    expect(rows.length).toBe(allCars.length + 1); // +1 header
  });
});

test('filters table data by name', async () => {
  (CarRepository.getAll as jest.Mock).mockImplementation((filterName) => {
    return Promise.resolve(allCars.filter(car => car.name.toLowerCase().includes(filterName.toLowerCase())));
  });

  render(<HomePage />);

  const filterButton = screen.getAllByRole('button').find(btn => btn.className.includes("filter"));
  fireEvent.click(filterButton!);

  const input = await screen.findByPlaceholderText('Search Name');
  fireEvent.change(input, { target: { value: 'tesla' } });
  fireEvent.keyDown(input, { key: 'Enter', code: 'Enter' });

  await waitFor(() => {
    expect(screen.getByText('Tesla')).toBeInTheDocument();
    expect(screen.queryByText('BMW')).not.toBeInTheDocument();
    expect(screen.queryByText('Audi')).not.toBeInTheDocument();
  });
});

test('clears filter when search input is empty', async () => {
  (CarRepository.getAll as jest.Mock).mockImplementation((filterName) => {
    return Promise.resolve(
      filterName === '' ? allCars : allCars.filter(car => car.name.toLowerCase().includes(filterName.toLowerCase()))
    );
  });

  render(<HomePage />);
  const filterButton = screen.getAllByRole('button').find(btn => btn.className.includes("filter"));
  fireEvent.click(filterButton!);

  const input = await screen.findByPlaceholderText('Search Name');
  fireEvent.change(input, { target: { value: '' } });
  fireEvent.keyDown(input, { key: 'Enter', code: 'Enter' });

  await waitFor(() => {
    expect(screen.getByText('Tesla')).toBeInTheDocument();
    expect(screen.getByText('BMW')).toBeInTheDocument();
    expect(screen.getByText('Audi')).toBeInTheDocument();
  });
});

test('shows no results if search does not match any car', async () => {
  (CarRepository.getAll as jest.Mock).mockResolvedValue([]);

  render(<HomePage />);
  const filterButton = screen.getAllByRole('button').find(btn => btn.className.includes("filter"));
  fireEvent.click(filterButton!);

  const input = await screen.findByPlaceholderText('Search Name');
  fireEvent.change(input, { target: { value: 'unknown' } });
  fireEvent.keyDown(input, { key: 'Enter', code: 'Enter' });

  await waitFor(() => {
    expect(screen.queryByText('Tesla')).not.toBeInTheDocument();
    expect(screen.queryByText('BMW')).not.toBeInTheDocument();
    expect(screen.queryByText('Audi')).not.toBeInTheDocument();
  });
});

test('should highlight the most expensive and least expensive cars', async () => {
  render(<HomePage />);
  await waitFor(() => {
    const mostExpensive = screen.getByText('Tesla').closest('tr');
    const leastExpensive = screen.getByText('Audi').closest('tr');

    expect(mostExpensive).toHaveClass('most-expensive');
    expect(leastExpensive).toHaveClass('least-expensive');
  });
});
