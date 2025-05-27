import { Button, Form, Input, InputNumber, Select, message } from 'antd';
import './UpdatePage.css';
import { Car, CarRepository } from '../DataSource';
import { useState } from 'react';

export const UpdatePage = () => {
    const [car, setCar] = useState<Car | null>(null);

    const fetchCarByName = async (name: string) => {
        const foundCar = (await CarRepository.getAll()).find(c => c.name === name);
        if (foundCar) {
            setCar(foundCar);
        } else {
            setCar(null);
            message.error('Car not found!');
        }
    };

    const onFinish = (values: Car) => {
        if (!car) return;

        CarRepository.update({
            ...car, 
            ...values,
        });

        message.success('Car updated successfully!');
    };

    const onFinishFailed = (errorInfo: any) => {
        console.log('Failed:', errorInfo);
    };

    return (
        <div className='formWrapper'>
            <Form
                className='formCard'
                name="updateForm"
                labelCol={{ span: 4 }}
                wrapperCol={{ span: 16 }}
                style={{ maxWidth: 600 }}
                onFinish={onFinish}
                onFinishFailed={onFinishFailed}
                autoComplete="off"
            >
                <Form.Item
                    label="Name"
                    name="name"
                    rules={[{ required: true, message: 'Please input the car name!' }]}
                >
                    <Input onBlur={(e) => fetchCarByName(e.target.value)} />
                </Form.Item>

                {car && (
                    <>
                        <Form.Item
                            label="Brand"
                            name="brand"
                            initialValue={car.brand}
                            rules={[{ required: true, message: 'Please input your brand!' }]}
                        >
                            <Input />
                        </Form.Item>

                        <Form.Item
                            label="Model"
                            name="model"
                            initialValue={car.model}
                            rules={[{ required: true, message: 'Please input your model!' }]}
                        >
                            <Input />
                        </Form.Item>

                        <Form.Item
                            label="Year"
                            name="year"
                            initialValue={car.year}
                            rules={[{ required: true, message: 'Please input your year!' }]}
                        >
                            <InputNumber min={1900} max={new Date().getFullYear()} />
                        </Form.Item>

                        <Form.Item
                            label="Color"
                            name="color"
                            initialValue={car.color}
                            rules={[{ required: true, message: 'Please input your color!' }]}
                        >
                            <Select>
                                <Select.Option value="Red">Red</Select.Option>
                                <Select.Option value="Blue">Blue</Select.Option>
                                <Select.Option value="Green">Green</Select.Option>
                                <Select.Option value="Black">Black</Select.Option>
                                <Select.Option value="White">White</Select.Option>
                                <Select.Option value="Silver">Silver</Select.Option>
                            </Select>
                        </Form.Item>

                        <Form.Item
                            label="Price"
                            name="price"
                            initialValue={car.price}
                            rules={[{ required: true, message: 'Please input your price!' }]}
                        >
                            <InputNumber min={0} />
                        </Form.Item>
                    </>
                )}

                <Form.Item className='submitButtonWrapper' label={null}>
                    <Button className='submitButton' type="primary" htmlType="submit" disabled={!car}>
                        Update
                    </Button>
                </Form.Item>
            </Form>
        </div>
    );
};
