import { render, screen, fireEvent, waitFor, act } from '@testing-library/react';
import { AddPage } from '../pages/AddPage';
import { Car, CarRepository } from '../DataSource';

jest.mock('../DataSource', () => ({
  CarRepository: {
    add: jest.fn(),
    getAll: jest.fn(),
  },
}));

const mockCars: Car[] = [
  { id: "1", key: "car1", name: 'Tesla', brand: 'Tesla', model: 'Model S', year: 2022, color: 'Red', price: 100000 },
  { id: "2", key: "car2", name: 'BMW', brand: 'BMW', model: 'X5', year: 2020, color: 'Black', price: 80000 },
  { id: "3", key: "car3", name: 'Audi', brand: 'Audi', model: 'A6', year: 2021, color: 'White', price: 70000 }
];

describe('AddPage', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    (CarRepository.getAll as jest.Mock).mockResolvedValue([]);
    (CarRepository.add as jest.Mock).mockImplementation();
    (CarRepository.add as jest.MockedFunction<typeof CarRepository.add>).mockResolvedValue(undefined);
  });

  test('renders the form with the correct fields', () => {
    render(<AddPage />);

    expect(screen.getByLabelText('Name')).toBeInTheDocument();
    expect(screen.getByLabelText('Brand')).toBeInTheDocument();
    expect(screen.getByLabelText('Model')).toBeInTheDocument();
    expect(screen.getByLabelText('Year')).toBeInTheDocument();
    expect(screen.getByLabelText('Color')).toBeInTheDocument();
    expect(screen.getByLabelText('Price')).toBeInTheDocument();
  });

  test('submits the form with valid data', async () => {
  
    render(<AddPage />);
  
    fireEvent.change(screen.getByLabelText('Name'), { target: { value: 'Tesla' } });
    fireEvent.change(screen.getByLabelText('Brand'), { target: { value: 'Tesla' } });
    fireEvent.change(screen.getByLabelText('Model'), { target: { value: 'Model S' } });
    fireEvent.change(screen.getByLabelText('Year'), { target: { value: '2022' } });
  
    fireEvent.mouseDown(screen.getByLabelText('Color'));
    await waitFor(() => screen.getByText('Silver'));
    fireEvent.click(screen.getByText('Silver'));
  
    fireEvent.change(screen.getByLabelText('Price'), { target: { value: '100000' } });
  
    const addButton = screen.getByRole('button', { name: /Add Car/i });
    fireEvent.click(addButton);
  
    await waitFor(() => {
      expect(CarRepository.add).toHaveBeenCalledWith({
        key: 'car1',
        name: 'Tesla',
        brand: 'Tesla',
        model: 'Model S',
        color: 'Silver',
        year: 2022,
        price: 100000,
      });
    });
  });
  

  test('shows validation errors when form is not filled out correctly', async () => {
    render(<AddPage />);

    fireEvent.click(screen.getByText('Add Car'));

    await waitFor(() => {
      expect(screen.getByText('Please input the car name!')).toBeInTheDocument();
      expect(screen.getByText('Please input the car brand!')).toBeInTheDocument();
      expect(screen.getByText('Please input the car model!')).toBeInTheDocument();
      expect(screen.getByText('Please input the production year!')).toBeInTheDocument();
      expect(screen.getByText('Please select the car color!')).toBeInTheDocument();
      expect(screen.getByText('Please input the car price!')).toBeInTheDocument();
    });
  });

  test('does not submit the form when any required field is empty', async () => {
    render(<AddPage />);

    await act(async () => {
      fireEvent.change(screen.getByLabelText('Brand'), { target: { value: 'Tesla' } });
      fireEvent.change(screen.getByLabelText('Model'), { target: { value: 'Model X' } });
      fireEvent.change(screen.getByLabelText('Year'), { target: { value: '2022' } });

      fireEvent.change(screen.getByLabelText('Price'), { target: { value: '90000' } });

      fireEvent.click(screen.getByText('Add Car'));
    });

    await waitFor(() => {
      expect(CarRepository.add).not.toHaveBeenCalled();
    });
  });
});
