// Trata se a URL termina com "/" para evitar "//calcular"
const BASE_URL = (window.ENV_CONFIG?.API_BASE_URL || '').replace(/\/$/, '');

// 1. POST para enviar o cálculo
async function calculate() {
    const num1 = document.getElementById('num1').value;
    const num2 = document.getElementById('num2').value;
    const operation = document.getElementById('operation').value;
    const resultElement = document.getElementById('result');

    if (!num1 || !num2) {
        resultElement.innerText = "Preencha os números!";
        resultElement.style.color = "red";
        return;
    }

    resultElement.innerText = "Calculando...";
    resultElement.style.color = "#333";

    // Estrutura JSON ajustada para sua API
    const payload = {
        leftOperand: parseFloat(num1),
        operator: operation,
        rightOperand: parseFloat(num2)
    };

    try {
        const response = await fetch(`${BASE_URL}/calcular`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        });

        if (!response.ok) throw new Error('Erro ao calcular');

        const data = await response.json();

        // Exibe o resultado retornado do backend
        resultElement.innerText = data.result ?? data;
        resultElement.style.color = "#28a745";

        loadHistory();

    } catch (error) {
        console.error(error);
        resultElement.innerText = "Erro na requisição";
        resultElement.style.color = "red";
    }
}

// 2. GET para trazer o histórico
async function loadHistory() {
    const historyList = document.getElementById('historyList');

    try {
        const response = await fetch(`${BASE_URL}/historico`, {
            method: 'GET'
        });

        if (!response.ok) throw new Error('Erro ao buscar histórico');

        const history = await response.json();
        historyList.innerHTML = '';

        if (Array.isArray(history) && history.length > 0) {
            history.forEach(item => {
                const li = document.createElement('li');
                // Formata conforme as propriedades retornadas pela sua API
                li.innerText = typeof item === 'object'
                    ? `${item.leftOperand} ${item.operator} ${item.rightOperand} = ${item.result}`
                    : item;
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