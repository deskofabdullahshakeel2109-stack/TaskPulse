import { createContext, useContext, useEffect, useState } from "react";
import authService from "../services/authService";

const AuthContext = createContext();

export function AuthProvider({ children }) {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const token = localStorage.getItem("taskpulse_token");

        if (!token) {
            setLoading(false);
            return;
        }

        authService
            .me()
            .then((userData) => {
                setUser(userData);
            })
            .catch(() => {
                localStorage.removeItem("taskpulse_token");
                setUser(null);
            })
            .finally(() => {
                setLoading(false);
            });
    }, []);

    const login = async (data) => {
        const response = await authService.login(data);

        const userData = await authService.me();
        setUser(userData);

        return response;
    };

    const register = async (data) => {
        return await authService.register(data);
    };

    const logout = () => {
        authService.logout();
        setUser(null);
    };

    return (
        <AuthContext.Provider
            value={{
                user,
                loading,
                login,
                register,
                logout,
                isAuthenticated: !!user,
            }}
        >
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    return useContext(AuthContext);
}