const API_URL = '/api';
let currentProjectId = null;

// При завантаженні сторінки одразу отримуємо список проєктів
document.addEventListener('DOMContentLoaded', () => {
    loadProjects();
});

// ================= ПРОЄКТИ =================

async function loadProjects() {
    try {
        const response = await fetch(`${API_URL}/projects`);
        const projects = await response.json();

        const list = document.getElementById('projectsList');
        list.innerHTML = '';

        projects.forEach(p => {
            const li = document.createElement('li');
            // Стилізуємо обраний проект по-іншому
            const isSelected = p.id === currentProjectId;
            li.className = `p-3 rounded-lg cursor-pointer transition-colors shadow-sm border 
                ${isSelected ? 'bg-blue-50 border-blue-300 border-l-4 border-l-blue-600' : 'bg-white border-gray-100 hover:bg-gray-50'}`;

            li.innerHTML = `
                <div class="flex justify-between items-center" onclick="selectProject(${p.id}, '${p.name.replace(/'/g, "\\'")}', '${(p.description || '').replace(/'/g, "\\'")}')">
                    <div>
                        <h3 class="font-bold ${isSelected ? 'text-blue-800' : 'text-gray-800'}">${p.name}</h3>
                    </div>
                </div>
                <div class="mt-2 text-right">
                     <button onclick="deleteProject(${p.id}, event)" class="text-xs text-red-500 hover:text-red-700 bg-red-50 px-2 py-1 rounded">Видалити</button>
                </div>
            `;
            list.appendChild(li);
        });
    } catch (error) {
        console.error('Помилка завантаження проєктів:', error);
    }
}

async function createProject(event) {
    event.preventDefault(); // Зупиняємо перезавантаження сторінки
    const name = document.getElementById('projName').value;
    const desc = document.getElementById('projDesc').value;

    const response = await fetch(`${API_URL}/projects`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ name: name, description: desc })
    });

    if (response.ok) {
        closeModal('projectModal');
        document.getElementById('formCreateProject').reset();
        loadProjects();
    }
}

async function deleteProject(id, event) {
    event.stopPropagation(); // Щоб не спрацював клік вибору проекту
    if (!confirm('Ви впевнені, що хочете видалити цей проєкт?')) return;

    await fetch(`${API_URL}/projects/${id}`, { method: 'DELETE' });

    if (currentProjectId === id) {
        currentProjectId = null;
        document.getElementById('kanbanBoard').classList.add('hidden');
        document.getElementById('noProjectSelectedMsg').classList.remove('hidden');
        document.getElementById('btnAddTask').classList.add('hidden');
        document.getElementById('currentProjectTitle').innerText = 'Оберіть проєкт зліва';
        document.getElementById('currentProjectDesc').innerText = '';
    }
    loadProjects();
}

function selectProject(id, name, desc) {
    currentProjectId = id;

    // Оновлюємо UI
    document.getElementById('currentProjectTitle').innerText = name;
    document.getElementById('currentProjectDesc').innerText = desc || 'Немає опису';

    document.getElementById('noProjectSelectedMsg').classList.add('hidden');
    document.getElementById('kanbanBoard').classList.remove('hidden');
    document.getElementById('btnAddTask').classList.remove('hidden');

    loadProjects(); // Перемальовуємо ліве меню для підсвітки
    loadTasks();    // Завантажуємо таски обраного проекту
}


// ================= ЗАВДАННЯ (TASKS) =================

async function loadTasks() {
    if (!currentProjectId) return;

    const response = await fetch(`${API_URL}/projects/${currentProjectId}/tasks`);
    const tasks = await response.json();

    // Очищаємо колонки
    document.getElementById('tasks-status-0').innerHTML = ''; // Не розпочате
    document.getElementById('tasks-status-1').innerHTML = ''; // В процесі
    document.getElementById('tasks-status-2').innerHTML = ''; // Завершене

    tasks.forEach(task => {
        const card = createTaskCard(task);
        document.getElementById(`tasks-status-${task.status}`).appendChild(card);
    });
}

function createTaskCard(task) {
    const div = document.createElement('div');
    div.className = 'task-card bg-white p-4 rounded-lg shadow border border-gray-100 flex flex-col gap-2';

    // Кольори для пріоритетів
    const priorityColors = ['bg-green-100 text-green-800', 'bg-yellow-100 text-yellow-800', 'bg-red-100 text-red-800'];
    const priorityLabels = ['Низький', 'Середній', 'Високий'];

    div.innerHTML = `
        <div class="flex justify-between items-start">
            <span class="text-xs font-bold px-2 py-1 rounded ${priorityColors[task.priority]}">${priorityLabels[task.priority]}</span>
            <div class="flex gap-1">
                <button onclick="openEditTaskModal(${task.id}, '${task.title}', '${task.description || ''}', ${task.priority})" class="text-gray-400 hover:text-blue-600">✎</button>
                <button onclick="deleteTask(${task.id})" class="text-gray-400 hover:text-red-600">×</button>
            </div>
        </div>
        <h4 class="font-bold text-gray-800 mt-1">${task.title}</h4>
        <p class="text-sm text-gray-500">${task.description || ''}</p>
        
        <div class="mt-3 flex gap-2 justify-end border-t pt-2 border-gray-50">
            ${task.status > 0 ? `<button onclick="changeTaskStatus(${task.id}, ${task.status - 1})" class="text-xs text-gray-500 hover:text-gray-800 bg-gray-100 px-2 py-1 rounded">◀ Назад</button>` : ''}
            ${task.status < 2 ? `<button onclick="changeTaskStatus(${task.id}, ${task.status + 1})" class="text-xs text-white bg-blue-500 hover:bg-blue-600 px-2 py-1 rounded">Далі ▶</button>` : ''}
        </div>
    `;
    return div;
}

async function saveTask(event) {
    event.preventDefault();
    if (!currentProjectId) return;

    const taskId = document.getElementById('taskId').value;
    const dto = {
        title: document.getElementById('taskTitle').value,
        description: document.getElementById('taskDesc').value,
        priority: parseInt(document.getElementById('taskPriority').value)
    };

    if (taskId) {
        // Оновлення (PUT)
        await fetch(`${API_URL}/projects/${currentProjectId}/tasks/${taskId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dto)
        });
    } else {
        // Створення (POST)
        await fetch(`${API_URL}/projects/${currentProjectId}/tasks`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dto)
        });
    }

    closeModal('taskModal');
    loadTasks();
}

async function deleteTask(taskId) {
    if (!confirm('Видалити завдання?')) return;
    await fetch(`${API_URL}/projects/${currentProjectId}/tasks/${taskId}`, { method: 'DELETE' });
    loadTasks();
}

async function changeTaskStatus(taskId, newStatus) {
    await fetch(`${API_URL}/projects/${currentProjectId}/tasks/${taskId}/status`, {
        method: 'PUT', // Ми використовували PUT для статусу в контролері
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ newStatus: newStatus })
    });
    loadTasks();
}

// ================= МОДАЛЬНІ ВІКНА =================

function openModal(modalId) {
    if (modalId === 'taskModal') {
        document.getElementById('formTask').reset();
        document.getElementById('taskId').value = '';
        document.getElementById('taskModalTitle').innerText = 'Нове завдання';
    }
    document.getElementById(modalId).classList.remove('hidden');
}

function openEditTaskModal(id, title, desc, priority) {
    document.getElementById('taskId').value = id;
    document.getElementById('taskTitle').value = title;
    document.getElementById('taskDesc').value = desc;
    document.getElementById('taskPriority').value = priority;
    document.getElementById('taskModalTitle').innerText = 'Редагувати завдання';
    document.getElementById('taskModal').classList.remove('hidden');
}

function closeModal(modalId) {
    document.getElementById(modalId).classList.add('hidden');
}
