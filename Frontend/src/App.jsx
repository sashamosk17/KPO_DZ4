import React, { useEffect, useState } from 'react';

const ORDERS_API = 'http://localhost:5001';
const PAYMENTS_API = 'http://localhost:5002';

export default function App() {
    const [userId, setUserId] = useState('123e4567-e89b-12d3-a456-426614174000');
    const [orders, setOrders] = useState([]);
    const [balance, setBalance] = useState(null);
    const [amount, setAmount] = useState(150);
    const [description, setDescription] = useState('Тестовый заказ');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const fetchData = async (currentUserId = userId) => {
        if (!currentUserId) return;
        setError('');
        try {
            const [ordersRes, accountRes] = await Promise.all([
                fetch(`${ORDERS_API}/api/orders/by-user/${currentUserId}`),
                fetch(`${PAYMENTS_API}/api/accounts/${currentUserId}`)
            ]);

            if (!ordersRes.ok) {
                const txt = await ordersRes.text();
                throw new Error(`Ошибка загрузки заказов: ${ordersRes.status} ${txt}`);
            }
            if (!accountRes.ok) {
                const txt = await accountRes.text();
                throw new Error(`Ошибка загрузки счета: ${accountRes.status} ${txt}`);
            }

            const ordersData = await ordersRes.json();
            const accountData = await accountRes.json();

            setOrders(ordersData);
            setBalance(accountData.balance);
        } catch (e) {
            setError(e.message);
            setOrders([]);
            setBalance(null);
        }
    };

    useEffect(() => {
        fetchData();
    }, []);

    const handleChangeUserId = e => {
        const value = e.target.value;
        setUserId(value);
    };

    const handleLoadForUser = e => {
        e.preventDefault();
        fetchData(userId);
    };

    const createOrder = async e => {
        e.preventDefault();
        if (!userId) {
            setError('Сначала введите userId');
            return;
        }
        setLoading(true);
        setError('');
        try {
            const res = await fetch(`${ORDERS_API}/api/orders`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    userId,
                    amount: Number(amount),
                    description
                })
            });

            if (!res.ok) {
                const txt = await res.text();
                throw new Error(`Не удалось создать заказ: ${res.status} ${txt}`);
            }

            await fetchData(userId);
        } catch (e) {
            console.error(e);
            setError(e.message);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ fontFamily: 'sans-serif', maxWidth: 800, margin: '20px auto' }}>
            <h1>Orders & Payments</h1>

            <section>
                <h2>Пользователь</h2>
                <form onSubmit={handleLoadForUser}>
                    <label>
                        UserId:&nbsp;
                        <input
                            type="text"
                            value={userId}
                            onChange={handleChangeUserId}
                            style={{ width: '100%' }}
                        />
                    </label>
                    <button type="submit" style={{ marginTop: 8 }}>
                        Загрузить данные
                    </button>
                </form>
            </section>

            <section>
                <h2>Баланс</h2>
                <p>Текущий баланс: {balance ?? '—'}</p>
            </section>

            <section>
                <h2>Новый заказ</h2>
                <form onSubmit={createOrder}>
                    <div>
                        <label>
                            Сумма:&nbsp;
                            <input
                                type="number"
                                step="0.01"
                                value={amount}
                                onChange={e => setAmount(e.target.value)}
                            />
                        </label>
                    </div>
                    <div>
                        <label>
                            Описание:&nbsp;
                            <input
                                type="text"
                                value={description}
                                onChange={e => setDescription(e.target.value)}
                            />
                        </label>
                    </div>
                    <button type="submit" disabled={loading}>
                        {loading ? 'Создаю...' : 'Создать заказ'}
                    </button>
                </form>
            </section>

            <section>
                <h2>Мои заказы</h2>
                {error && <p style={{ color: 'red' }}>{error}</p>}
                <table border="1" cellPadding="4" cellSpacing="0">
                    <thead>
                        <tr>
                            <th>Id</th>
                            <th>Сумма</th>
                            <th>Описание</th>
                            <th>Статус</th>
                        </tr>
                    </thead>
                    <tbody>
                        {orders.map(o => (
                            <tr key={o.id}>
                                <td>{o.id}</td>
                                <td>{o.amount}</td>
                                <td>{o.description}</td>
                                <td>{o.status}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </section>
        </div>
    );
}
