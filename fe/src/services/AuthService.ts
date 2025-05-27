import axios from 'axios';

const apiUrl = process.env.REACT_APP_API_URL;

export class AuthService {
  static async login(username: string, password: string): Promise<string> {
    const res = await axios.post(`/auth/login`, { username, password });
    const token = res.data.token;
    localStorage.setItem('jwt', token);
    return token;
  }

  static async register(username: string, password: string): Promise<void> {
    await axios.post(`/auth/register`, { username, password });
  }
}
