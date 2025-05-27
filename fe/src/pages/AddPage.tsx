import { Button, Form, FormProps, Input, InputNumber, Select, message } from 'antd';
import './AddPage.css';
import { Car, CarRepository } from '../DataSource';

export const AddPage = () => {
    const [form] = Form.useForm<Car>();

    const onFinish: FormProps<Car>['onFinish'] = async (values) => {
        const existingCars = await CarRepository.getAll();
        const nextId = existingCars.length + 1;

        const newCar: Car = {
            ...values,
            key: `car${nextId}`
        };

        await CarRepository.add(newCar);
        message.success(`Car "${values.name}" added successfully!`);
        form.resetFields();
    };

    const onFinishFailed: FormProps<Car>['onFinishFailed'] = (errorInfo) => {
        message.error('Failed to add car. Please check the form for errors.');
        console.error('Add car error:', errorInfo);
    };

    return (
        <div className='formWrapper'>
            <Form
                className='formCard'
                form={form}
                name="add-car-form"
                labelCol={{ span: 4 }}
                wrapperCol={{ span: 16 }}
                style={{ maxWidth: 600 }}
                onFinish={onFinish}
                onFinishFailed={onFinishFailed}
                autoComplete="off"
            >
                <Form.Item<Car>
                    label="Name"
                    name="name"
                    rules={[{ required: true, message: 'Please input the car name!' }]}
                >
                    <Input />
                </Form.Item>

                <Form.Item<Car>
                    label="Brand"
                    name="brand"
                    rules={[{ required: true, message: 'Please input the car brand!' }]}
                >
                    <Input />
                </Form.Item>

                <Form.Item<Car>
                    label="Model"
                    name="model"
                    rules={[{ required: true, message: 'Please input the car model!' }]}
                >
                    <Input />
                </Form.Item>

                <Form.Item<Car>
                    label="Year"
                    name="year"
                    rules={[{ required: true, message: 'Please input the production year!' }]}
                >
                    <InputNumber
                        min={1900}
                        max={new Date().getFullYear()}
                        style={{ width: '100%' }}
                    />
                </Form.Item>

                <Form.Item<Car>
                    label="Color"
                    name="color"
                    rules={[{ required: true, message: 'Please select the car color!' }]}
                >
                    <Select placeholder="Select color">
                        <Select.Option value="Red">Red</Select.Option>
                        <Select.Option value="Blue">Blue</Select.Option>
                        <Select.Option value="Green">Green</Select.Option>
                        <Select.Option value="Black">Black</Select.Option>
                        <Select.Option value="White">White</Select.Option>
                        <Select.Option value="Silver">Silver</Select.Option>
                    </Select>
                </Form.Item>

                <Form.Item<Car>
                    label="Price"
                    name="price"
                    rules={[{ required: true, message: 'Please input the car price!' }]}
                >
                    <InputNumber min={0} style={{ width: '100%' }} />
                </Form.Item>

                <Form.Item className='submitButtonWrapper' labelCol={{ span: 0 }} wrapperCol={{ span: 24 }}>
                    <Button className='submitButton' type="primary" htmlType="submit">
                        Add Car
                    </Button>
                </Form.Item>
            </Form>
        </div>
    );
};
