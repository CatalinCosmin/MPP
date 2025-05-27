import { Button, Form, FormProps, Input, Popconfirm, message } from 'antd';
import './DeletePage.css';
import { Car, CarRepository } from '../DataSource';
import { useState } from 'react';

export const DeletePage = () => {
    const [car, setCar] = useState<Car | null>(null);
    const [form] = Form.useForm();

    const fetchCarByName = async (name: string) => {
        const foundCar = (await CarRepository.getAll()).find(c => c.name === name);
        if (foundCar) {
            setCar(foundCar);
        } else {
            setCar(null);
            message.error('Car not found!');
        }
    };

    const onFinish: FormProps<Car>['onFinish'] = (values) => {
        const itemToDelete = values.name;
        
        if (car) {
            CarRepository.delete(itemToDelete);
            message.success('Item deleted');
            form.resetFields();
            setCar(null);
        }
    };

    const onFinishFailed: FormProps<Car>['onFinishFailed'] = (errorInfo) => {
        console.log('Validation failed:', errorInfo);
    };

    return (
        <div className="formWrapper">
            <Form
                className="formCard"
                name="deleteForm"
                labelCol={{ span: 4 }}
                wrapperCol={{ span: 16 }}
                style={{ maxWidth: 600 }}
                form={form}
                onFinish={onFinish}
                onFinishFailed={onFinishFailed}
                autoComplete="off"
            >
                <Form.Item
                    label="Name"
                    name="name"
                    rules={[{ required: true, message: 'Please input the name of the item to delete!' }]}
                >
                    <Input onBlur={(e) => fetchCarByName(e.target.value)} />
                </Form.Item>

                <Form.Item className="submitButtonWrapper" label={null}>
                    <Popconfirm
                        title="Are you sure you want to delete this item?"
                        onConfirm={() => form.submit()} // Trigger form submission to validate and delete
                        onCancel={() => message.info('Delete action cancelled')}
                        okText="Yes"
                        cancelText="No"
                    >
                        <Button
                            className="submitButton"
                            type="primary"
                            disabled={!car} // Disable button if car is not found
                        >
                            Delete
                        </Button>
                    </Popconfirm>
                </Form.Item>
            </Form>
        </div>
    );
};
