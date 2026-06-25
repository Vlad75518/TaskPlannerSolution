/**
 * ══════════════════════════════════════════════════════════
 * TaskPlanner — Клієнтська логіка  (app.js)
 * ══════════════════════════════════════════════════════════
 *
 * Цей файл — єдиний JS-файл рівня представлення (PL).
 * Його єдина відповідальність: взаємодіяти з користувачем
 * та відправляти/отримувати дані через API-запити.
 * Жодної бізнес-логіки тут немає.
 *
 * ──────────────────────────────────────────────────────────
 * СТРУКТУРА ФАЙЛУ
 * ──────────────────────────────────────────────────────────
 *  § 1  Стан та конфігурація
 *  § 2  API-модуль        — fetch-запити до сервера
 *  § 3  Рендеринг         — побудова DOM на основі даних
 *  § 4  Drag & Drop       — перетягування карток між колонками
 *  § 5  Обробники дій     — реакції на кліки та форми
 *  § 6  Модальні вікна    — відкриття, закриття, форми
 *  § 7  Ініціалізація     — точка входу
 */


/* ════════════════════════════════════════════════════════
   § 1  СТАН ТА КОНФІГУРАЦІЯ
   ════════════════════════════════════════════════════════ */

/** Базовий URL для всіх API-запитів */
const API_BASE = '/api';

/**
 * Центральний об'єкт стану застосунку.
 * Всі змінні стану зберігаються тут — жодних глобальних змінних.
 */
const state = {
    /** ID поточного обраного проєкту (null = нічого не обрано) */
    currentProjectId: null,

    /** ID картки, яку зараз перетягують (null = немає) */
    draggedTaskId: null,

    /** Статус колонки, з якої почалося перетягування */
    draggedFromStatus: null,
};

/**
 * Метадані для відображення пріоритетів.
 * Порядок відповідає числовим значенням enum TaskPriority.
 */
const PRIORITY = [
    { label: 'Низький', badge: 'badge-low' },
    { label: 'Середній', badge: 'badge-medium' },
    { label: 'Високий', badge: 'badge-high' },
];


/* ════════════════════════════════════════════════════════
   § 2  API-МОДУЛЬ
   Всі звернення до сервера зосереджені тут.
   Компоненти рендерингу ніколи не роблять fetch напряму.
   ════════════════════════════════════════════════════════ */

/**
 * Допоміжна функція: виконує запит із JSON-тілом.
 * @param {string} url
 * @param {'POST'|'PUT'|'DELETE'} method
 * @param {object} [body]
 * @returns {Promise<Response>}
 */
async function apiRequest(url, method, body) {
    return fetch(url, {
        method,
        headers: { 'Content-Type': 'application/json' },
        body: body !== undefined ? JSON.stringify(body) : undefined,
    });
}

// ── Проєкти ───────────────────────────────────────────

/** Отримати список усіх проєктів. */
async function fetchProjects() {
    const res = await fetch(`${API_BASE}/projects`);
    if (!res.ok) throw new Error('Не вдалося завантажити проєкти');
    return res.json();
}

/** Створити новий проєкт. */
async function apiCreateProject(name, description) {
    return apiRequest(`${API_BASE}/projects`, 'POST', { name, description });
}

async function apiUpdateProject(id, name, description) {
    return apiRequest(`${API_BASE}/projects/${id}`, 'PUT', { name, description });
}

/** Видалити проєкт за ID. */
async function apiDeleteProject(id) {
    return apiRequest(`${API_BASE}/projects/${id}`, 'DELETE');
}

// ── Завдання ──────────────────────────────────────────

/** Отримати всі завдання конкретного проєкту. */
async function fetchTasks(projectId) {
    const res = await fetch(`${API_BASE}/projects/${projectId}/tasks`);
    if (!res.ok) throw new Error('Не вдалося завантажити завдання');
    return res.json();
}

/** Створити нове завдання у проєкті. */
async function apiCreateTask(projectId, dto) {
    return apiRequest(`${API_BASE}/projects/${projectId}/tasks`, 'POST', dto);
}

/** Оновити завдання (назва, опис, пріоритет). */
async function apiUpdateTask(projectId, taskId, dto) {
    return apiRequest(`${API_BASE}/projects/${projectId}/tasks/${taskId}`, 'PUT', dto);
}

/**
 * Змінити статус завдання — API для переміщення між колонками.
 * @param {number} projectId
 * @param {number} taskId
 * @param {0|1|2} newStatus  — 0=NotStarted, 1=InProgress, 2=Completed
 */
async function apiChangeStatus(projectId, taskId, newStatus) {
    return apiRequest(
        `${API_BASE}/projects/${projectId}/tasks/${taskId}/status`,
        'PUT',
        { newStatus }
    );
}

/** Видалити завдання. */
async function apiDeleteTask(projectId, taskId) {
    return apiRequest(`${API_BASE}/projects/${projectId}/tasks/${taskId}`, 'DELETE');
}


/* ════════════════════════════════════════════════════════
   § 3  РЕНДЕРИНГ
   Функції що будують DOM-дерево на основі даних від API.
   Принцип: один fetch → повний перемальовуємо блок.
   ════════════════════════════════════════════════════════ */

/**
 * Завантажити та відобразити список проєктів у боковій панелі.
 * Підсвічує поточний активний проєкт.
 */
async function renderProjects() {
    let projects;
    try {
        projects = await fetchProjects();
    } catch (err) {
        console.error('[TaskPlanner] renderProjects:', err);
        return;
    }

    const list = document.getElementById('projects-list');
    list.innerHTML = '';

    if (projects.length === 0) {
        const hint = document.createElement('li');
        hint.className = 'projects-empty-hint';
        hint.textContent = 'Немає проєктів. Створіть перший!';
        list.appendChild(hint);
        return;
    }

    projects.forEach(project => {
        const li = createProjectItem(project);
        list.appendChild(li);
    });
}

/**
 * Створити DOM-елемент одного рядка у списку проєктів.
 * @param {{ id: number, name: string, description?: string }} project
 * @returns {HTMLLIElement}
 */
function createProjectItem(project) {
    const li = document.createElement('li');
    li.className = `project-item${project.id === state.currentProjectId ? ' active' : ''}`;
    li.dataset.projectId = project.id;
    li.setAttribute('role', 'button');
    li.setAttribute('tabindex', '0');
    li.setAttribute('aria-label', `Проєкт: ${project.name}`);

    const nameSpan = document.createElement('span');
    nameSpan.className = 'project-item-name';
    nameSpan.textContent = project.name;

    const actionsDiv = document.createElement('div');
    actionsDiv.style.display = 'flex';
    actionsDiv.style.gap = '2px';

    const editBtn = document.createElement('button');
    editBtn.className = 'project-action-btn';
    editBtn.title = 'Редагувати проєкт';
    editBtn.textContent = '✎';
    editBtn.addEventListener('click', e => {
        e.stopPropagation();
        openEditProjectModal(project);
    });

    const deleteBtn = document.createElement('button');
    deleteBtn.className = 'project-action-btn delete';
    deleteBtn.title = 'Видалити проєкт';
    deleteBtn.textContent = '×';
    deleteBtn.addEventListener('click', e => {
        e.stopPropagation();
        handleDeleteProject(project.id);
    });

    actionsDiv.appendChild(editBtn);
    actionsDiv.appendChild(deleteBtn);

    li.appendChild(nameSpan);
    li.appendChild(actionsDiv);

    // Клік по рядку = вибрати цей проєкт
    li.addEventListener('click', () => selectProject(project));

    // Підтримка клавіатури (Enter / Space)
    li.addEventListener('keydown', e => {
        if (e.key === 'Enter' || e.key === ' ') {
            e.preventDefault();
            selectProject(project);
        }
    });

    return li;
}

/**
 * Завантажити та відобразити завдання поточного проєкту.
 * Розподіляє картки по трьох колонках за полем status.
 */
async function renderTasks() {
    if (!state.currentProjectId) return;

    let tasks;
    try {
        tasks = await fetchTasks(state.currentProjectId);
    } catch (err) {
        console.error('[TaskPlanner] renderTasks:', err);
        return;
    }

    // Очищуємо всі колонки
    const counts = [0, 0, 0];
    for (let s = 0; s <= 2; s++) {
        document.getElementById(`tasks-status-${s}`).innerHTML = '';
    }

    // Розкидаємо картки по відповідних колонках
    tasks.forEach(task => {
        const col = document.getElementById(`tasks-status-${task.status}`);
        if (!col) return;
        col.appendChild(createTaskCard(task));
        counts[task.status]++;
    });

    // Оновлюємо лічильники у заголовках колонок
    counts.forEach((count, status) => {
        document.getElementById(`count-${status}`).textContent = count;
    });

    // Підказка для порожніх колонок
    for (let s = 0; s <= 2; s++) {
        const col = document.getElementById(`tasks-status-${s}`);
        if (counts[s] === 0) {
            const hint = document.createElement('p');
            hint.className = 'column-empty-hint';
            hint.textContent = 'Перетягніть сюди завдання';
            col.appendChild(hint);
        }
    }
}

/**
 * Побудувати DOM-елемент картки завдання.
 * @param {{ id, title, description, priority, status }} task
 * @returns {HTMLDivElement}
 */
function createTaskCard(task) {
    const priority = PRIORITY[task.priority] ?? PRIORITY[0];

    const card = document.createElement('div');
    card.className = 'task-card';
    card.dataset.taskId = task.id;
    card.dataset.status = task.status;
    card.setAttribute('role', 'listitem');

    // ── Drag & Drop ──────────────────────────────────
    card.draggable = true;
    card.addEventListener('dragstart', e => handleDragStart(e, task.id, task.status));
    card.addEventListener('dragend', handleDragEnd);

    // ── Верхній рядок: бейдж пріоритету + кнопки ────
    const top = document.createElement('div');
    top.className = 'card-top';

    const badge = document.createElement('span');
    badge.className = `priority-badge ${priority.badge}`;
    badge.textContent = priority.label;

    const actions = document.createElement('div');
    actions.className = 'card-actions';

    // Кнопка «Редагувати»
    const editBtn = document.createElement('button');
    editBtn.className = 'card-action-btn';
    editBtn.title = 'Редагувати завдання';
    editBtn.setAttribute('aria-label', `Редагувати «${task.title}»`);
    editBtn.innerHTML = `
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor"
         stroke-width="2" width="14" height="14" aria-hidden="true">
      <path d="M11 4H4a2 2 0 00-2 2v14a2 2 0 002 2h14a2 2 0 002-2v-7"/>
      <path d="M18.5 2.5a2.121 2.121 0 013 3L12 15l-4 1 1-4 9.5-9.5z"/>
    </svg>`;
    editBtn.addEventListener('click', () => openEditTaskModal(task));

    // Кнопка «Видалити»
    const delBtn = document.createElement('button');
    delBtn.className = 'card-action-btn delete';
    delBtn.title = 'Видалити завдання';
    delBtn.setAttribute('aria-label', `Видалити «${task.title}»`);
    delBtn.innerHTML = `
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor"
         stroke-width="2" width="14" height="14" aria-hidden="true">
      <polyline points="3 6 5 6 21 6"/>
      <path d="M19 6l-1 14a2 2 0 01-2 2H8a2 2 0 01-2-2L5 6"/>
      <path d="M10 11v6M14 11v6"/>
    </svg>`;
    delBtn.addEventListener('click', () => handleDeleteTask(task.id));

    actions.appendChild(editBtn);
    actions.appendChild(delBtn);
    top.appendChild(badge);
    top.appendChild(actions);

    // ── Назва ────────────────────────────────────────
    const titleEl = document.createElement('h3');
    titleEl.className = 'card-title';
    titleEl.textContent = task.title;  // textContent — безпечно, без XSS

    card.appendChild(top);
    card.appendChild(titleEl);

    // ── Опис (якщо є) ─────────────────────────────────
    if (task.description) {
        const descEl = document.createElement('p');
        descEl.className = 'card-desc';
        descEl.textContent = task.description;
        card.appendChild(descEl);
    }

    return card;
}


/* ════════════════════════════════════════════════════════
   § 4  DRAG & DROP
   Перетягування карток між колонками канбану.
   Використовує стандартний HTML5 Drag and Drop API.

   Послідовність подій:
     dragstart  → user починає тягнути картку
     dragover   → курсор над drop-зоною (колонкою)
     dragleave  → курсор виходить з drop-зони
     drop       → user відпустив картку над колонкою
     dragend    → завершення (в будь-якому випадку)
   ════════════════════════════════════════════════════════ */

/**
 * Початок перетягування.
 * Зберігаємо, яку картку і з якої колонки тягнемо.
 */
function handleDragStart(event, taskId, fromStatus) {
    state.draggedTaskId = taskId;
    state.draggedFromStatus = fromStatus;

    event.currentTarget.classList.add('is-dragging');
    event.dataTransfer.effectAllowed = 'move';
    event.dataTransfer.setData('text/plain', String(taskId)); // потрібно для Firefox
}

/**
 * Курсор над drop-зоною: дозволяємо скидання та підсвічуємо колонку.
 */
function handleDragOver(event) {
    event.preventDefault();
    event.dataTransfer.dropEffect = 'move';
    event.currentTarget.classList.add('drag-over');
}

/**
 * Курсор вийшов з drop-зони — прибираємо підсвітку.
 * Перевіряємо relatedTarget щоб уникнути миготіння при переході
 * між дочірніми елементами колонки.
 */
function handleDragLeave(event) {
    if (!event.currentTarget.contains(event.relatedTarget)) {
        event.currentTarget.classList.remove('drag-over');
    }
}

/**
 * Скидання картки на колонку.
 * Якщо колонка змінилась — надсилаємо запит на зміну статусу.
 * @param {DragEvent} event
 * @param {0|1|2} newStatus — статус колонки куди скинули
 */
async function handleDrop(event, newStatus) {
    event.preventDefault();
    event.currentTarget.classList.remove('drag-over');

    const { draggedTaskId: taskId, draggedFromStatus: fromStatus } = state;

    // Скинули в ту саму колонку — нічого не робимо
    if (taskId === null || fromStatus === newStatus) return;

    try {
        await apiChangeStatus(state.currentProjectId, taskId, newStatus);
        await renderTasks(); // перемальовуємо дошку з новими даними
    } catch (err) {
        console.error('[TaskPlanner] handleDrop:', err);
    }
}

/**
 * Кінець перетягування (незалежно від результату).
 * Прибираємо CSS-клас і скидаємо стан.
 */
function handleDragEnd(event) {
    event.currentTarget.classList.remove('is-dragging');

    // Прибираємо підсвітку з усіх колонок про всяк випадок
    document.querySelectorAll('.column-body').forEach(col => {
        col.classList.remove('drag-over');
    });

    state.draggedTaskId = null;
    state.draggedFromStatus = null;
}


/* ════════════════════════════════════════════════════════
   § 5  ОБРОБНИКИ ДІЙ
   Реакції на кліки — з'єднують UI з API-модулем.
   ════════════════════════════════════════════════════════ */

/**
 * Вибрати проєкт: оновити стан і перемалювати потрібні блоки.
 * @param {{ id, name, description }} project
 */
function selectProject(project) {
    state.currentProjectId = project.id;

    // Оновлюємо шапку основного вмісту
    document.getElementById('current-project-title').textContent = project.name;
    document.getElementById('current-project-desc').textContent = project.description || '';

    // Переключаємо стан UI: ховаємо «порожній стан», показуємо канбан
    document.getElementById('empty-state').classList.add('hidden');
    document.getElementById('kanban-board').classList.remove('hidden');
    document.getElementById('btn-add-task').classList.remove('hidden');

    // Підсвічуємо обраний рядок у списку (без повного перерендеру)
    document.querySelectorAll('.project-item').forEach(item => {
        const isActive = Number(item.dataset.projectId) === project.id;
        item.classList.toggle('active', isActive);
    });

    // Завантажуємо завдання нового проєкту
    renderTasks();
}

/**
 * Видалити проєкт після підтвердження від користувача.
 * @param {number} id
 */
async function handleDeleteProject(id) {
    if (!confirm('Видалити цей проєкт разом із усіма завданнями?')) return;

    try {
        await apiDeleteProject(id);
    } catch (err) {
        console.error('[TaskPlanner] handleDeleteProject:', err);
        return;
    }

    // Якщо видалили поточний проєкт — повертаємося до порожнього стану
    if (state.currentProjectId === id) {
        state.currentProjectId = null;
        document.getElementById('current-project-title').textContent = 'Оберіть проєкт';
        document.getElementById('current-project-desc').textContent = '';
        document.getElementById('empty-state').classList.remove('hidden');
        document.getElementById('kanban-board').classList.add('hidden');
        document.getElementById('btn-add-task').classList.add('hidden');
    }

    // Оновлюємо список проєктів
    renderProjects();
}

/**
 * Обробник форми «Новий проєкт».
 * Викликається кнопкою «Створити» в модальному вікні.
 */
async function handleCreateProject() {
    const id = document.getElementById('proj-id').value;
    const name = document.getElementById('proj-name').value.trim();
    if (!name) {
        document.getElementById('proj-name').focus();
        return;
    }
    const description = document.getElementById('proj-desc').value.trim();

    try {
        if (id) {
            // Редагування
            await apiUpdateProject(id, name, description);
            if (state.currentProjectId === Number(id)) {
                document.getElementById('current-project-title').textContent = name;
                document.getElementById('current-project-desc').textContent = description;
            }
        } else {
            // Створення
            await apiCreateProject(name, description);
        }
        closeModal('project');
        document.getElementById('proj-id').value = '';
        document.getElementById('proj-name').value = '';
        document.getElementById('proj-desc').value = '';
        await renderProjects();
    } catch (err) {
        console.error('[TaskPlanner] handleCreateProject:', err);
    }
}

/**
 * Обробник форми «Зберегти завдання».
 * Створює нове завдання або оновлює існуюче (залежно від task-id).
 */
async function handleSaveTask() {
    const title = document.getElementById('task-title').value.trim();
    if (!title) {
        document.getElementById('task-title').focus();
        return;
    }

    const taskId = document.getElementById('task-id').value;
    const dto = {
        title,
        description: document.getElementById('task-desc').value.trim(),
        priority: parseInt(document.getElementById('task-priority').value, 10),
    };

    try {
        if (taskId) {
            // Режим редагування → PUT
            await apiUpdateTask(state.currentProjectId, Number(taskId), dto);
        } else {
            // Режим створення → POST
            await apiCreateTask(state.currentProjectId, dto);
        }
        closeModal('task');
        await renderTasks();
    } catch (err) {
        console.error('[TaskPlanner] handleSaveTask:', err);
    }
}

/**
 * Видалити завдання після підтвердження.
 * @param {number} taskId
 */
async function handleDeleteTask(taskId) {
    if (!confirm('Видалити це завдання?')) return;

    try {
        await apiDeleteTask(state.currentProjectId, taskId);
        await renderTasks();
    } catch (err) {
        console.error('[TaskPlanner] handleDeleteTask:', err);
    }
}


/* ════════════════════════════════════════════════════════
   § 6  МОДАЛЬНІ ВІКНА
   Відкриття, закриття, заповнення форм.
   ════════════════════════════════════════════════════════ */

/**
 * Відкрити модальне вікно.
 * @param {'project'|'task'} key
 */
function openModal(key) {
    // При відкритті вікна «нового завдання» — скидаємо форму
    if (key === 'task' && !document.getElementById('task-id').value) {
        document.getElementById('task-id').value = '';
        document.getElementById('task-title').value = '';
        document.getElementById('task-desc').value = '';
        selectPriority(1); // за замовчуванням — Середній
        document.getElementById('modal-task-heading').textContent = 'Нове завдання';
    }

    const overlay = document.getElementById(`modal-${key}`);
    overlay.classList.remove('hidden');

    // Автофокус на перший текстовий ввід
    const firstInput = overlay.querySelector('.form-input');
    if (firstInput) {
        setTimeout(() => firstInput.focus(), 50); // невелика затримка після анімації
    }
}

/**
 * Закрити модальне вікно.
 * @param {'project'|'task'} key
 */
function closeModal(key) {
    document.getElementById(`modal-${key}`).classList.add('hidden');
    // Скидаємо task-id щоб наступне відкриття було в режимі "новий"
    if (key === 'task') {
        document.getElementById('task-id').value = '';
    }
}

/**
 * Закрити модальне вікно при кліку на затемнений фон (overlay).
 * Ігноруємо кліки по самій картці модального вікна.
 * @param {MouseEvent} event
 * @param {'project'|'task'} key
 */
function closeModalOnOverlay(event, key) {
    if (event.target === event.currentTarget) {
        closeModal(key);
    }
}

/**
 * Відкрити модальне вікно редагування завдання з передзаповненими полями.
 * @param {{ id, title, description, priority }} task
 */
function openEditTaskModal(task) {
    document.getElementById('task-id').value = task.id;
    document.getElementById('task-title').value = task.title ?? '';
    document.getElementById('task-desc').value = task.description ?? '';
    selectPriority(task.priority ?? 1);
    document.getElementById('modal-task-heading').textContent = 'Редагувати завдання';
    openModal('task');
}

/**
 * Вибрати пріоритет у модальному вікні завдання.
 * Підсвічує відповідну кнопку та оновлює прихований input.
 * @param {0|1|2} value
 */
function selectPriority(value) {
    // Зберігаємо числове значення у прихованому input
    document.getElementById('task-priority').value = value;

    // Перемикаємо активний клас на кнопках
    document.querySelectorAll('.priority-btn').forEach(btn => {
        const isActive = parseInt(btn.dataset.value, 10) === value;
        btn.classList.toggle('priority-btn--active', isActive);
        btn.setAttribute('aria-checked', isActive ? 'true' : 'false');
    });
}

// Відкрити модалку редагування проєкту
function openEditProjectModal(project) {
    document.getElementById('proj-id').value = project.id;
    document.getElementById('proj-name').value = project.name;
    document.getElementById('proj-desc').value = project.description || '';
    document.getElementById('modal-project-heading').textContent = 'Редагувати проєкт';
    openModal('project');
}

// Скидання модалки проєкту при новому відкритті
const originalOpenModal = openModal;
openModal = function (key) {
    if (key === 'project' && !document.getElementById('proj-id').value) {
        document.getElementById('proj-name').value = '';
        document.getElementById('proj-desc').value = '';
        document.getElementById('modal-project-heading').textContent = 'Новий проєкт';
    }
    originalOpenModal(key);
}

// Закрити дошку
function closeBoard() {
    state.currentProjectId = null;
    document.getElementById('current-project-title').textContent = 'Оберіть проєкт';
    document.getElementById('current-project-desc').textContent = '';
    document.getElementById('empty-state').classList.remove('hidden');
    document.getElementById('kanban-board').classList.add('hidden');
    document.getElementById('btn-add-task').classList.add('hidden');
    document.getElementById('btn-close-board').classList.add('hidden');

    document.querySelectorAll('.project-item').forEach(item => item.classList.remove('active'));
}

// Зміна видимості кнопки закриття в selectProject
const originalSelectProject = selectProject;
selectProject = function (project) {
    originalSelectProject(project);
    document.getElementById('btn-close-board').classList.remove('hidden');
}

// ГЛОБАЛЬНИЙ ПОШУК
let searchTimeout = null;
async function handleGlobalSearch(e) {
    const query = e.target.value.trim();
    const dropdown = document.getElementById('search-results');

    if (searchTimeout) clearTimeout(searchTimeout);

    if (query.length < 2) {
        dropdown.classList.add('hidden');
        return;
    }

    searchTimeout = setTimeout(async () => {
        try {
            const res = await fetch(`${API_BASE}/search/tasks?q=${encodeURIComponent(query)}`);
            if (!res.ok) return;
            const tasks = await res.json();

            dropdown.innerHTML = '';
            if (tasks.length === 0) {
                dropdown.innerHTML = '<div class="search-item" style="color: var(--text-muted)">Нічого не знайдено</div>';
            } else {
                tasks.forEach(task => {
                    const item = document.createElement('div');
                    item.className = 'search-item';
                    item.innerHTML = `<div style="font-weight:600">${task.title}</div><div style="font-size:11px; color:var(--text-muted)">Статус: ${task.status === 0 ? 'Не розпочато' : task.status === 1 ? 'У виконанні' : 'Завершено'}</div>`;

                    item.onclick = async () => {
                        dropdown.classList.add('hidden');
                        document.getElementById('global-search').value = '';
                        // Знаходимо і перемикаємо на проєкт
                        const projects = await fetchProjects();
                        const p = projects.find(pr => pr.id === task.projectId);
                        if (p) selectProject(p);
                    };
                    dropdown.appendChild(item);
                });
            }
            dropdown.classList.remove('hidden');
        } catch (err) { }
    }, 300);
}

// Ховаємо пошук при кліку поза ним
document.addEventListener('click', e => {
    if (!e.target.closest('.search-wrapper')) {
        document.getElementById('search-results')?.classList.add('hidden');
    }
});


/* ════════════════════════════════════════════════════════
   § 7  ІНІЦІАЛІЗАЦІЯ
   Виконується після того як DOM повністю завантажений.
   ════════════════════════════════════════════════════════ */

document.addEventListener('DOMContentLoaded', () => {

    // ── Завантажуємо список проєктів ──────────────────
    renderProjects();

    // ── Клавіша Escape закриває будь-яке відкрите вікно ─
    document.addEventListener('keydown', e => {
        if (e.key === 'Escape') {
            closeModal('project');
            closeModal('task');
        }
    });

    // ── Enter у полі назви проєкту = зберегти ─────────
    document.getElementById('proj-name').addEventListener('keydown', e => {
        if (e.key === 'Enter') handleCreateProject();
    });

    // ── Enter у полі назви завдання = зберегти ────────
    document.getElementById('task-title').addEventListener('keydown', e => {
        if (e.key === 'Enter') handleSaveTask();
    });

});
