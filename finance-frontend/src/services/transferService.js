import { apiRequest } from './api';

export function createTransfer(transfer) {
    return apiRequest('/api/transfers', {
        method: 'POST',
        body: JSON.stringify(transfer)
    });
}

export function cancelTransfer(id) {
    return apiRequest(`/api/transfers/${id}`, {
        method: 'DELETE'
    });
}

export function getTransfers() {
    return apiRequest('/api/transfers');
}
