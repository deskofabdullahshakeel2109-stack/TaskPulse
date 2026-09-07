import axios from "axios";

const axiosClient = axios.create({
    baseURL: "https://localhost:7237/api",
    headers: {
        "Content-Type": "application/json",
    },
});

// Automatically attach JWT token
axiosClient.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem("taskpulse_token");

        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }

        return config;
    },
    (error) => Promise.reject(error)
);

export default axiosClient;