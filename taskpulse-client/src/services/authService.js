import axiosClient from "../api/axiosClient";

const authService = {
    register: async (data) => {
        const response = await axiosClient.post("/auth/register", data);
        return response.data;
    },

    login: async (data) => {
        const response = await axiosClient.post("/auth/login", data);

        const token = response.data.token;

        if (token) {
            localStorage.setItem("taskpulse_token", token);
        }

        return response.data;
    },

    me: async () => {
        const response = await axiosClient.get("/auth/me");
        return response.data;
    },

    logout: () => {
        localStorage.removeItem("taskpulse_token");
    },
};

export default authService;