// Имитация сервера (асинхронная загрузка)
const MOCK_BOOKS = [
    { id: 1, title: "Мастер и Маргарита", author: "Михаил Булгаков", available: true },
    { id: 2, title: "1984", author: "Джордж Оруэлл", available: false },
    { id: 3, title: "Преступление и наказание", author: "Фёдор Достоевский", available: true },
    { id: 4, title: "Евгений Онегин", author: "Александр Сергеевич Пушкин", available: false },
    { id: 5, title: "Великий Гэтсби", author: "Фрэнсис Скотт Фицджеральд", available: true },
    { id: 6, title: "Война и мир", author: "Лев Николаевич Толстой", available: false },
    { id: 7, title: "На западном фронте без перемен", author: "Эрих Мария Ремарк", available: true },
    { id: 8, title: "Ведьмак", author: "Анджей Сапковский", available: true }
];

const fetchBooksFromServer = () => {
    return new Promise((resolve) => {
        setTimeout(() => {
            resolve([...MOCK_BOOKS]);
        }, 1200);
    });
};

// Компонент BookItem (через React.createElement)
const BookItem = ({ book }) => {
    const isAvailable = book.available;
    const statusClass = isAvailable ? "status-available" : "status-unavailable";
    const statusText = isAvailable ? "В наличии" : "Нет в наличии";
    const statusIcon = isAvailable ? "✓" : "✕";

    return React.createElement(
        "div",
        { className: "book-card" },
        React.createElement("div", { className: "book-cover-placeholder" }, "📖"),
        React.createElement(
            "div",
            { className: "book-info" },
            React.createElement("div", { className: "book-title" }, book.title),
            React.createElement(
                "div",
                { className: "book-author" },
                React.createElement("span", { className: "author-icon" }, "✍️"),
                " ",
                book.author
            ),
            React.createElement(
                "div",
                { className: `status-badge ${statusClass}` },
                React.createElement("span", { className: "badge-icon" }, statusIcon),
                React.createElement("span", null, statusText)
            )
        )
    );
};

// Главный компонент App
const App = () => {
    const [books, setBooks] = React.useState([]);
    const [loading, setLoading] = React.useState(true);
    const [filterStatus, setFilterStatus] = React.useState('all');

    React.useEffect(() => {
        let isMounted = true;
        const loadBooks = async () => {
            setLoading(true);
            try {
                const data = await fetchBooksFromServer();
                if (isMounted) setBooks(data);
            } catch (error) {
                console.error("Ошибка загрузки:", error);
                if (isMounted) setBooks([]);
            } finally {
                if (isMounted) setLoading(false);
            }
        };
        loadBooks();
        return () => { isMounted = false; };
    }, []);

    const filteredBooks = React.useMemo(() => {
        if (!books.length) return [];
        switch (filterStatus) {
            case 'available':
                return books.filter(book => book.available === true);
            case 'unavailable':
                return books.filter(book => book.available === false);
            default:
                return books;
        }
    }, [books, filterStatus]);

    const handleFilterChange = (status) => {
        setFilterStatus(status);
    };

    const renderContent = () => {
        if (loading) {
            return React.createElement(
                "div",
                { className: "loading-state" },
                React.createElement("div", { className: "spinner" }),
                React.createElement("div", { className: "loading-text" }, "Загрузка книг из библиотеки...")
            );
        }

        if (filteredBooks.length === 0) {
            let emptyMessage = "Нет книг, соответствующих фильтру.";
            if (books.length === 0) emptyMessage = "Библиотека временно пуста. Попробуйте позже.";
            return React.createElement(
                "div",
                { className: "empty-state" },
                React.createElement("div", { className: "empty-icon" }, "📭"),
                React.createElement("div", null, emptyMessage),
                filterStatus !== 'all' && books.length > 0 && React.createElement(
                    "button",
                    {
                        onClick: () => handleFilterChange('all'),
                        style: { marginTop: '1rem', background: '#eef2fa', border: 'none', padding: '0.5rem 1.2rem', borderRadius: '2rem', cursor: 'pointer', fontWeight: 500 }
                    },
                    "Показать все книги"
                )
            );
        }

        return React.createElement(
            React.Fragment,
            null,
            React.createElement(
                "div",
                { className: "result-stats" },
                React.createElement(
                    "span",
                    { className: "counter" },
                    "Найдено: ",
                    filteredBooks.length,
                    " ",
                    filteredBooks.length === 1 ? "книга" : "книг(и)"
                )
            ),
            React.createElement(
                "div",
                { className: "books-grid" },
                filteredBooks.map(book => React.createElement(BookItem, { key: book.id, book: book }))
            )
        );
    };

    return React.createElement(
        "div",
        { className: "library-container" },
        React.createElement(
            "div",
            { className: "catalog-header" },
            React.createElement("h1", null, "📚 Онлайн-библиотека"),
            React.createElement("div", { className: "subhead" }, "Каталог книг с индикацией доступности")
        ),
        React.createElement(
            "div",
            { className: "filter-bar" },
            React.createElement(
                "button",
                {
                    className: `filter-btn ${filterStatus === 'all' ? 'active' : ''}`,
                    onClick: () => handleFilterChange('all')
                },
                "Все книги"
            ),
            React.createElement(
                "button",
                {
                    className: `filter-btn ${filterStatus === 'available' ? 'active' : ''}`,
                    onClick: () => handleFilterChange('available')
                },
                "✅ Доступные"
            ),
            React.createElement(
                "button",
                {
                    className: `filter-btn ${filterStatus === 'unavailable' ? 'active' : ''}`,
                    onClick: () => handleFilterChange('unavailable')
                },
                "❌ Недоступные"
            )
        ),
        renderContent(),
        React.createElement(
            "div",
            { style: { textAlign: 'center', fontSize: '0.7rem', marginTop: '2rem', color: '#7893b0' } },
            "⚡ Данные загружены асинхронно (имитация сервера)"
        )
    );
};

// Рендеринг приложения
const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(React.createElement(App));
