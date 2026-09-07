import axiosClient from "../api/axiosClient";

const dashboardService = {
    getProjectDashboard: async (projectId) => {
        const response = await axiosClient.get(
            `/projects/${projectId}/dashboard`
        );

        return response.data;
    },
};

export default dashboardService;