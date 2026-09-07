import axiosClient from "../api/axiosClient";

const taskService = {
    getByProject: async (projectId, params = {}) => {
        const response = await axiosClient.get(
            `/projects/${projectId}/tasks`,
            { params }
        );

        return response.data;
    },

    getById: async (projectId, taskId) => {
        const response = await axiosClient.get(
            `/projects/${projectId}/tasks/${taskId}`
        );

        return response.data;
    },

    create: async (projectId, data) => {
        const response = await axiosClient.post(
            `/projects/${projectId}/tasks`,
            {
                ...data,
                projectId,
            }
        );

        return response.data;
    },

    update: async (projectId, taskId, data) => {
        const response = await axiosClient.put(
            `/projects/${projectId}/tasks/${taskId}`,
            data
        );

        return response.data;
    },

    delete: async (projectId, taskId) => {
        const response = await axiosClient.delete(
            `/projects/${projectId}/tasks/${taskId}`
        );

        return response.data;
    },
};

export default taskService;