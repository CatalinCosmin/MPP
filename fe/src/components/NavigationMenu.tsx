import { Menu } from "antd";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../pages/AuthContext";
import './NavigationMenu.css';

export const NavigationMenu = () => {
  const navigate = useNavigate();
  const { isAuthenticated, role, logout } = useAuth();

  const handleClick = (e: any) => {
    if (e.key === "logout") {
      logout();
      navigate("/login");
    } else {
      navigate(e.key);
    }
  };

  const items = [];

  if (isAuthenticated) {
    items.push(
      { label: "Home", key: "home" },
      { label: "Add", key: "add" },
      { label: "Update", key: "update" },
      { label: "Delete", key: "delete" },
      { label: "Metrics", key: "metrics" }
    );
    if (role === "Admin") {
      items.push({ label: "Admin", key: "admin" });
    }
    items.push({ label: "Logout", key: "logout" });
  } else {
    items.push({ label: "Login", key: "login" }, { label: "Register", key: "register" });
  }

  return (
    <Menu className="navBar" onClick={handleClick} mode="horizontal" items={items} />
  );
};
