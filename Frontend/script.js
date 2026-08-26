// Trata se a URL termina com "/" para evitar "//calcular"
const BASE_URL = (window.ENV_CONFIG?.API_BASE_URL || '').replace(/\/$/, '');

const display = document.getElementById('display');
let list = [];

function append(value) {
    if (display.value === 'Erro') display.value = '';
    display.value += value;
}

function clearDisplay() {
    display.value = '';
}

// 1. POST para enviar o cálculo
async function calculate() {
    if (!display.value) return;

    // Envia a string inteira digitada no display como 'expression'
    const payload = {
        expression: display.value
    };

    try {
        const response = await fetch(`${BASE_URL}/calcular`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (!response.ok) throw new Error();

        const data = await response.json();
        display.value = data.result;

        loadHistory();
    } catch {
        display.value = 'Erro';
    }
}

async function loadHistory() {
    const historyList = document.getElementById('historyList');

    try {
        const response = await fetch(`${BASE_URL}/historico`, { method: 'GET' });
        if (!response.ok) throw new Error('Erro ao buscar histórico');

        const history = await response.json();
        historyList.innerHTML = '';

        if (Array.isArray(history) && history.length > 0) {
            history.forEach(item => {
                const li = document.createElement('li');

                // 1. Exibe a expressão inteira se ela existir
                if (item.expression) {
                    li.innerText = `${item.expression} = ${item.result}`;
                }
                // 2. Exibe no formato simples caso seja um cálculo direto
                else if (item.leftOperand !== null && item.leftOperand !== undefined) {
                    li.innerText = `${item.leftOperand} ${item.operator} ${item.rightOperand} = ${item.result}`;
                }
                // 3. Fallback apenas com o resultado
                else {
                    li.innerText = `Resultado: ${item.result}`;
                }

                historyList.appendChild(li);
            });
        } else {
            historyList.innerHTML = '<li>Nenhum histórico encontrado.</li>';
        }
    } catch (error) {
        console.error(error);
        historyList.innerHTML = '<li style="color:red;">Erro ao carregar histórico</li>';
    }
}
// 3. DELETE Excluir Histórico
async function clearHistory() {
    try {
        const response = await fetch(`${BASE_URL}/historico`, {
            method: 'DELETE'
        });

        if (response.ok) {
            // Se o backend deletou com sucesso, recarrega a lista vazia ou limpa a tela
            await loadHistory();
            // Alternativa direta sem nova requisição HTTP:
            // document.getElementById('historyList').innerHTML = '';
        } else {
            showError('Não foi possível excluir o histórico.');
        }
    } catch (error) {
        console.error('Erro:', error);
        showError('Erro ao excluir o histórico.');
    }
}