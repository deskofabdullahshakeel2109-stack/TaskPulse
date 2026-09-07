import axiosClient from "../api/axiosClient";

const projectService = {
    getAll: async () => {
        const response = await axiosClient.get("/projects");
        return response.data;
    },

    getById: async (id) => {
        const response = await axiosClient.get(`/projects/${id}`);
        return response.data;
    },

    create: async (data) => {
        const response = await axiosClient.post("/projects", data);
        return response.data;
    },

    update: async (id, data) => {
        const response = await axiosClient.put(`/projects/${id}`, data);
        return response.data;
    },

    delete: async (id) => {
        const response = await axiosClient.delete(`/projects/${id}`);
        return response.data;
    },
};

export default projectService;