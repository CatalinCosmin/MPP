import { AuthProvider, useAuth } from "./pages/AuthContext";
import { BrowserRouter, Route, Routes, Navigate } from "react-router-dom";
import { HomePage } from './pages/HomePage';
import { NavigationMenu } from './components/NavigationMenu';
import { AddPage } from './pages/AddPage';
import { UpdatePage } from './pages/UpdatePage';
import { DeletePage } from './pages/DeletePage';
import { MetricsPage } from './pages/MetricsPage';
import { AdminPage } from './pages/AdminPage';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { ConnectivityService } from './services/ConnectivityService';
import { CarRepository } from './DataSource';
import { OfflineQueue } from './services/OfflineQueue';
import { useEffect, useState } from "react";

const ProtectedRoute = ({ element, allowed }: { element: JSX.Element, allowed: boolean }) => {
  return allowed ? element : <Navigate to="/login" />;
};

const AppContent = () => {
  const { isAuthenticated, role } = useAuth();
  const [networkDown, setNetworkDown] = useState(false);
  const [serverDown, setServerDown] = useState(false);

  useEffect(() => {
    let syncing = false;

    const checkConnection = async () => {
      const status = await ConnectivityService.isAvailable();
      setNetworkDown(!status.network);
      setServerDown(status.network && !status.server);

      if (status.server && !syncing) {
        syncing = true;
        await CarRepository.syncOfflineChanges();
        syncing = false;
      }
    };

    checkConnection();
    const interval = setInterval(checkConnection, 5000);
    window.addEventListener('online', checkConnection);

    return () => {
      clearInterval(interval);
      window.removeEventListener('online', checkConnection);
    };
  }, []);

  const renderConnectionMessage = () => {
    if (networkDown) {
      return <div style={bannerStyle('red')}>🔌 You are offline. Changes will be synced when you're back online.</div>;
    }
    if (serverDown) {
      return <div style={bannerStyle('orange')}>⚠️ Server is unreachable. Changes will sync once it's back.</div>;
    }
    return null;
  };

  const bannerStyle = (bgColor: string): React.CSSProperties => ({
    backgroundColor: bgColor,
    color: 'white',
    padding: '10px',
    textAlign: 'center',
    fontWeight: 'bold',
  });

  return (
    <>
      {renderConnectionMessage()}
      <BrowserRouter>
        <NavigationMenu />
        <Routes>
          <Route path='/' element={<Navigate to='/home' />} />
          <Route path='/home' element={<ProtectedRoute allowed={isAuthenticated} element={<HomePage />} />} />
          <Route path='/add' element={<ProtectedRoute allowed={isAuthenticated} element={<AddPage />} />} />
          <Route path='/update' element={<ProtectedRoute allowed={isAuthenticated} element={<UpdatePage />} />} />
          <Route path='/delete' element={<ProtectedRoute allowed={isAuthenticated} element={<DeletePage />} />} />
          <Route path='/metrics' element={<ProtectedRoute allowed={isAuthenticated} element={<MetricsPage />} />} />
          <Route path='/admin' element={<ProtectedRoute allowed={isAuthenticated && role === 'Admin'} element={<AdminPage />} />} />
          <Route path='/login' element={<LoginPage />} />
          <Route path='/register' element={<RegisterPage />} />
        </Routes>
      </BrowserRouter>
    </>
  );
};

const App = () => (
  <AuthProvider>
    <AppContent />
  </AuthProvider>
);

export default App;
