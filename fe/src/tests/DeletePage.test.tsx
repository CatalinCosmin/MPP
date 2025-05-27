import React from 'react';
import { render, screen, fireEvent, waitFor, act } from '@testing-library/react';
import { DeletePage } from '../pages/DeletePage';
import { CarRepository } from '../DataSource';
import userEvent from '@testing-library/user-event';

jest.mock('../DataSource', () => ({
  CarRepository: {
    getAll: jest.fn(),
    delete: jest.fn(),
  },
}));

beforeEach(() => {
  jest.clearAllMocks();
  (CarRepository.getAll as jest.Mock).mockResolvedValue([{ name: 'Tesla', price: 1000 }]);
});

test('renders the delete form', async () => {
  render(<DeletePage />);

  expect(screen.getByLabelText('Name')).toBeInTheDocument();
  expect(screen.getByText('Delete')).toBeInTheDocument();
});

test('displays disabled button when car is not found', async () => {
  render(<DeletePage />);

  const nameInput = screen.getByLabelText('Name');
  await userEvent.type(nameInput, 'NonExistentCar');
  fireEvent.blur(nameInput); 

  const deleteButton = await screen.findByText('Delete');
  expect(deleteButton.parentElement).toBeDisabled(); 
});

test('triggers delete action when confirmed', async () => {
  render(<DeletePage />);

  const input = screen.getByLabelText('Name');
  fireEvent.change(input, { target: { value: 'Tesla' } });

  await act(async () => {
    fireEvent.blur(input);
  });

  const deleteButton = screen.getByText('Delete');
  userEvent.click(deleteButton);

  const confirmButton = await screen.findByText('Yes');
  userEvent.click(confirmButton);

  await waitFor(() => {
    expect(CarRepository.delete).toHaveBeenCalledWith('Tesla');
  });
});

test('does not call delete when popconfirm is cancelled', async () => {
  render(<DeletePage />);

  const input = screen.getByLabelText('Name');
  fireEvent.change(input, { target: { value: 'Tesla' } });

  await act(async () => {
    fireEvent.blur(input);
  });

  const deleteButton = screen.getByText('Delete');
  userEvent.click(deleteButton);

  const cancelButton = await screen.findByText('No');
  userEvent.click(cancelButton);

  await waitFor(() => {
    expect(CarRepository.delete).not.toHaveBeenCalled();
  });
});
