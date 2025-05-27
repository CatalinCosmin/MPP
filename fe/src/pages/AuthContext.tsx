// src/context/AuthContext.tsx

import { createContext, useContext, useState, useEffect } from "react";

type AuthContextType = {
  isAuthenticated: boolean;
  role: string | null;
  login: (token: string) => void;
  logout: () => void;
};

const AuthContext = createContext<AuthContextType>({
  isAuthenticated: false,
  role: null,
  login: () => {},
  logout: () => {},
});

export const useAuth = () => useContext(AuthContext);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [role, setRole] = useState<string | null>(null);

  useEffect(() => {
    const token = localStorage.getItem("jwt");
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split(".")[1]));
        setRole(payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]);
        setIsAuthenticated(true);
      } catch {
        setIsAuthenticated(false);
        setRole(null);
      }
    }
  }, []);

  const login = (token: string) => {
    localStorage.setItem("jwt", token);
    const payload = JSON.parse(atob(token.split(".")[1]));
    setRole(payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]);
    setIsAuthenticated(true);
  };

  const logout = () => {
    localStorage.removeItem("jwt");
    setRole(null);
    setIsAuthenticated(false);
  };

  return (
    <AuthContext.Provider value={{ isAuthenticated, role, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};
