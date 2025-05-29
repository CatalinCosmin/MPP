import { useEffect, useState } from 'react';
import axios from 'axios';
import './AdminPage.css';

export const AdminPage = () => {
  const [monitoredUsers, setMonitoredUsers] = useState([]);

  useEffect(() => {
    const token = localStorage.getItem('jwt');
    axios.get(process.env.REACT_APP_API_URL + '/admin/monitored-users', {
      headers: { Authorization: `Bearer ${token}` }
    }).then(res => {
      setMonitoredUsers(res.data);
    }).catch(err => {
      console.error('Failed to fetch monitored users', err);
    });
  }, []);

  return (
    <div className="admin-container">
      <h2>Monitored Users</h2>
      <ul className="user-list">
        {monitoredUsers.map((user: any) => (
          <li key={user.userId}>
            <span>{user.userId}</span>
            <span>{new Date(user.detectedAt).toLocaleString()}</span>
          </li>
        ))}
      </ul>
    </div>
  );
};
