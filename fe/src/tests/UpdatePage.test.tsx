import { render, screen, fireEvent, waitFor, act } from '@testing-library/react';
import { UpdatePage } from '../pages/UpdatePage';
import { Car, CarRepository } from '../DataSource';
import userEvent from '@testing-library/user-event';

jest.mock('../DataSource', () => ({
  CarRepository: {
    getAll: jest.fn(),
    update: jest.fn(),
  },
}));

const mockCars: Car[] = [   
  { id: "1", key: 'car1', name: 'Tesla', brand: 'Tesla', model: 'Model S', year: 2022, color: 'Red', price: 100000 },
  { id: "2", key: 'car2', name: 'BMW', brand: 'BMW', model: 'X5', year: 2020, color: 'Black', price: 80000 },
  { id: "3", key: 'car3', name: 'Audi', brand: 'Audi', model: 'A6', year: 2021, color: 'White', price: 70000 },
];

beforeEach(() => {
  (CarRepository.getAll as jest.MockedFunction<typeof CarRepository.getAll>).mockResolvedValue(mockCars);
  (CarRepository.update as jest.MockedFunction<typeof CarRepository.update>).mockClear();
  (CarRepository.update as jest.MockedFunction<typeof CarRepository.update>).mockImplementation();
});

describe('UpdatePage', () => {
  test('renders the form with the correct fields', async () => {
    render(<UpdatePage />);

    const nameInput = screen.getByLabelText('Name');
    fireEvent.change(nameInput, { target: { value: 'Tesla' } });
    
      await act(async () => {
        fireEvent.blur(nameInput);
      });

    expect(screen.getByLabelText('Name')).toBeInTheDocument();
    expect(screen.getByLabelText('Brand')).toBeInTheDocument();
    expect(screen.getByLabelText('Model')).toBeInTheDocument();
    expect(screen.getByLabelText('Year')).toBeInTheDocument();
    expect(screen.getByLabelText('Price')).toBeInTheDocument();
  });test('submits the form with valid data', async () => {
    render(<UpdatePage />);
  
    const nameInput = screen.getByLabelText('Name');
    await userEvent.type(nameInput, 'Tesla');
    fireEvent.blur(nameInput);
  
    await waitFor(() => {
      expect(screen.getByLabelText('Brand')).toHaveValue('Tesla');
      expect(screen.getByLabelText('Model')).toHaveValue('Model S');
      expect(screen.getByLabelText('Year')).toHaveValue('2022');
      expect(screen.getByLabelText('Price')).toHaveValue('100000');
    });
  
    const brandInput = screen.getByLabelText('Brand');
    const modelInput = screen.getByLabelText('Model');
    const yearInput = screen.getByLabelText('Year');
    const priceInput = screen.getByLabelText('Price');
    const colorSelect = screen.getByLabelText('Color');
    const updateButton = screen.getByRole('button', { name: /update/i });
  
    await userEvent.clear(brandInput);
    await userEvent.type(brandInput, 'Tesla');
  
    await userEvent.clear(modelInput);
    await userEvent.type(modelInput, 'Model X');
  
    await userEvent.clear(yearInput);
    await userEvent.type(yearInput, '2022');
  
    await userEvent.clear(priceInput);
    await userEvent.type(priceInput, '90000');
  
    fireEvent.mouseDown(colorSelect);
    await waitFor(() => {
      expect(screen.getByText('Black')).toBeInTheDocument();
    });
    await userEvent.click(screen.getByText('Black'));
  
    await userEvent.click(updateButton);
  
    await waitFor(() => {
      expect(CarRepository.update).toHaveBeenCalledTimes(1);
      expect(CarRepository.update).toHaveBeenCalledWith(expect.objectContaining({
        id: '1',
        key: 'car1',
        name: 'Tesla',
        brand: 'Tesla',
        model: 'Model X',
        year: 2022,
        color: 'Black',
        price: 90000,
      }));
    });
  }, 10000);
  

  test('shows validation errors when form is not filled out correctly', async () => {
    render(<UpdatePage />);

    const nameInput = screen.getByLabelText('Name');
    fireEvent.change(nameInput, { target: { value: 'Tesla' } });
    
    await act(async () => {
        fireEvent.blur(nameInput);
        });
    userEvent.clear(screen.getByLabelText('Brand'));
    userEvent.clear(screen.getByLabelText('Model'));
    userEvent.clear(screen.getByLabelText('Year'));

    userEvent.click(screen.getByText('Update'));

    await waitFor(() => {
      expect(screen.getByText('Please input your brand!')).toBeInTheDocument();
      expect(screen.getByText('Please input your model!')).toBeInTheDocument();
      expect(screen.getByText('Please input your year!')).toBeInTheDocument();
    });
  });
});
