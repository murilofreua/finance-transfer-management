const API_URL = import.meta.env.VITE_API_URL;

export async function apiRequest(endpoint, options = {}) {
    const response = await fetch(`${API_URL}${endpoint}`, {
        headers: {
            'Content-Type': 'application/json',
            ...options.headers
        },
        ...options
    });

    if (!response.ok) {
        const error = await response.json().catch(() => null);

        throw new Error(
            error?.message || 'Ocorreu um erro ao comunicar com a API.'
        );
    }

    if (response.status === 204) {
        return null;
    }

    return response.json();
}
