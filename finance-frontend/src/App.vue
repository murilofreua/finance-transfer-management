<script setup>
import { ref } from 'vue';
import { createAccount } from './services/accountService';
import { createTransfer, getTransfers, cancelTransfer } from './services/transferService';
import { getAccounts } from './services/accountService';
import { onMounted } from 'vue';

const personName = ref('');
const balance = ref(0);
const overdraftLimit = ref(0);

const account = ref(null);
const error = ref('');
const loading = ref(false);

async function handleCreateAccount() {
  error.value = '';
  account.value = null;
  loading.value = true;

  try {
    account.value = await createAccount({
      personName: personName.value,
      balance: balance.value,
      overdraftLimit: overdraftLimit.value
    });

    await loadAccounts();

    personName.value = '';
    balance.value = 0;
    overdraftLimit.value = 0;
  } catch (err) {
    error.value = err.message;
  } finally {
    loading.value = false;
  }
}

const sourceAccountId = ref('');
const destinationAccountId = ref('');
const transferAmount = ref(0);
const scheduledAt = ref('');

const transferResult = ref(null);
const transferError = ref('');
const transferLoading = ref(false);

async function handleCreateTransfer() {
  transferError.value = '';
  transferResult.value = null;
  transferLoading.value = true;

  try {
    const transfer = {
      sourceAccountId: sourceAccountId.value,
      destinationAccountId: destinationAccountId.value,
      amount: transferAmount.value,
      scheduledAt: scheduledAt.value
        ? new Date(scheduledAt.value).toISOString()
        : null
    };

    transferResult.value = await createTransfer(transfer);

    await loadTransfers();
    await loadAccounts();

    sourceAccountId.value = '';
    destinationAccountId.value = '';
    transferAmount.value = 0;
    scheduledAt.value = '';
  } catch (err) {
    transferError.value = err.message;
  } finally {
    transferLoading.value = false;
  }
}

const accounts = ref([]);
const accountsError = ref('');

async function loadAccounts() {
  try {
    accounts.value = await getAccounts();
  } catch (err) {
    accountsError.value = err.message;
  }
}

const transfers = ref([]);
const transfersError = ref('');

async function loadTransfers() {
  try {
    transfers.value = await getTransfers();
  } catch (err) {
    transfersError.value = err.message;
  }
}

async function handleCancelTransfer(id) {
  try {
    await cancelTransfer(id);
    await loadTransfers();
    await loadAccounts();
  } catch (err) {
    transfersError.value = err.message;
  }
}

const refreshLoading = ref(false);

async function refreshData() {
    refreshLoading.value = true;

    try {
        await Promise.all([
            loadAccounts(),
            loadTransfers()
        ]);
    } finally {
        refreshLoading.value = false;
    }
}

function formatCurrency(value) {
    return Number(value).toLocaleString('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    });
}

function getAccountName(accountId) {
    const account = accounts.value.find(item => item.id === accountId);

    return account ? account.personName : accountId;
}

onMounted(() => {
  loadAccounts();
  loadTransfers();
});
</script>

<template>
  <div class="app">
    <header class="header">
      <div>
        <h1>Finance</h1>
        <p>Gerenciamento de contas e transferências</p>
      </div>
    </header>

    <main class="container">
      <section class="welcome">
        <h2>Bem-vindo ao Finance</h2>
        <p>Gerencie suas contas e realize transferências financeiras.</p>
      </section>

      <div class="dashboard">
        <section class="card">
          <h3>Criar conta</h3>
          <p>Cadastre uma nova conta bancária.</p>

          <form @submit.prevent="handleCreateAccount">
            <label>Nome do titular</label>
            <input
              v-model="personName"
              type="text"
              placeholder="Nome completo"
              required
            />

            <label>Saldo inicial</label>
            <input
              v-model.number="balance"
              type="number"
              min="0"
              step="0.01"
              required
            />

            <label>Limite do cheque especial</label>
            <input
              v-model.number="overdraftLimit"
              type="number"
              min="0"
              step="0.01"
              required
            />

            <button type="submit" :disabled="loading">
              {{ loading ? 'Criando...' : 'Criar conta' }}
            </button>
          </form>

          <p v-if="error" class="error">{{ error }}</p>

          <div v-if="account" class="result">
            <h4>Conta criada com sucesso!</h4>
            <p><strong>Titular:</strong> {{ account.personName }}</p>
            <p><strong>Identificador:</strong> {{ account.id }}</p>
            <p><strong>Saldo:</strong> R$ {{ account.balance }}</p>
            <p><strong>Cheque especial:</strong> R$ {{ account.overdraftLimit }}</p>
          </div>

          <div class="accounts-list">
            <h3>Contas cadastradas</h3>

            <p v-if="accountsError" class="error">
              {{ accountsError }}
            </p>

            <p v-else-if="accounts.length === 0" class="empty-message">
              Nenhuma conta cadastrada.
            </p>

            <div v-for="item in accounts" :key="item.id" class="account-item">
              <div>
                <strong>{{ item.personName }}</strong>
                <p>ID: {{ item.id }}</p>
              </div>

              <div class="account-details">
                <span>Saldo: {{ formatCurrency(item.balance) }}</span>
                <span>Cheque especial: {{ formatCurrency(item.overdraftLimit) }}</span>
                <span>Status: {{ item.status }}</span>
              </div>
            </div>
          </div>
        </section>

        <section class="card">
          <h3>Transferências</h3>
          <p>Realize transferências imediatas ou agendadas.</p>

          <form @submit.prevent="handleCreateTransfer">
            <label>Conta de origem</label>
            <select v-model="sourceAccountId" required>
              <option value="" disabled>Selecione a conta de origem</option>
              <option
                v-for="item in accounts.filter(a => a.status === 'Active')"
                :key="item.id"
                :value="item.id"
              >
                {{ item.personName }} - {{ item.id }}
              </option>
            </select>

            <label>Conta de destino</label>
            <select v-model="destinationAccountId" required>
              <option value="" disabled>Selecione a conta de destino</option>
              <option
                v-for="item in accounts.filter(a => a.status === 'Active')"
                :key="item.id"
                :value="item.id"
              >
                {{ item.personName }} - {{ item.id }}
              </option>
            </select>

            <label>Valor da transferência</label>
            <input
              v-model.number="transferAmount"
              type="number"
              min="0.01"
              step="0.01"
              required
            />

            <label>Agendar para (opcional)</label>
            <input
              v-model="scheduledAt"
              type="datetime-local"
            />

            <button type="submit" :disabled="transferLoading">
              {{ transferLoading ? 'Processando...' : 'Realizar transferência' }}
            </button>
          </form>

          <p v-if="transferError" class="error">
            {{ transferError }}
          </p>

          <div v-if="transferResult" class="result">
            <h4>Transferência registrada!</h4>
            <p><strong>ID:</strong> {{ transferResult.id }}</p>
            <p><strong>Valor:</strong> R$ {{ transferResult.amount }}</p>
            <p><strong>Status:</strong> {{ transferResult.status }}</p>
          </div>

          <div class="transfers-list">
            <h3>Histórico de transferências</h3>

            <p v-if="transfersError" class="error">
              {{ transfersError }}
            </p>

            <p v-else-if="transfers.length === 0" class="empty-message">
              Nenhuma transferência registrada.
            </p>

            <button
                class="refresh-button"
                @click="refreshData"
                :disabled="refreshLoading"
            >
                {{ refreshLoading ? 'Atualizando...' : 'Atualizar informações' }}
            </button>

            <div
              v-for="transfer in transfers"
              :key="transfer.id"
              class="transfer-item"
            >
              <div>
                <strong>{{ formatCurrency(transfer.amount) }}</strong>
                <p>ID: {{ transfer.id }}</p>
              </div>

              <div class="transfer-details">
                <span>
                    Status:
                    <strong :class="['status-badge', transfer.status.toLowerCase()]">
                        {{ transfer.status }}
                    </strong>
                </span>
                <span>
                    Solicitada em:
                    {{ new Date(transfer.createdAt).toLocaleString('pt-BR') }}
                </span>
                <span v-if="transfer.scheduledAt">
                    Data de execução:
                    {{ new Date(transfer.scheduledAt).toLocaleString('pt-BR') }}
                </span>
                <span>Origem: {{ getAccountName(transfer.sourceAccountId) }}</span>
                <span>Destino: {{ getAccountName(transfer.destinationAccountId) }}</span>
              </div>

              <button
                v-if="transfer.status === 'Scheduled'"
                class="cancel-button"
                @click="handleCancelTransfer(transfer.id)"
              >
                Cancelar transferência
              </button>
            </div>
          </div>
        </section>
      </div>
    </main>
  </div>
</template>
