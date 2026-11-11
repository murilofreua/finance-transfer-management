import { apiRequest } from './api';

export function getAccount(id) {
    return apiRequest(`/api/accounts/${id}`);
}

export function createAccount(account) {
    return apiRequest('/api/accounts', {
        method: 'POST',
        body: JSON.stringify(account)
    });
}

export function getAccounts() {
    return apiRequest('/api/accounts');
}
