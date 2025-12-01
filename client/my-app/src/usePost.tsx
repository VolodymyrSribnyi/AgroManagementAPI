import { useState } from 'react';

// TRequest - тип даних, що відправляються (для POST або PUT)
// TResponse - тип даних, що повертаються
interface UsePostResult<TResponse> {
    data: TResponse | null;
    loading: boolean;
    error: string | null;
    /**
     * Виконує POST (за замовчуванням) або PUT запит.
     * @param url URL-адреса ресурсу.
     * @param data Тіло запиту.
     * @param method HTTP-метод ('POST' для створення, 'PUT' для редагування/оновлення).
     */
    post: (url: string, data: any, method?: 'POST' | 'PUT') => Promise<void>;
}

/**
 * Custom React Hook для виконання запитів POST та PUT з логікою повторних спроб (Exponential Backoff).
 * Підходить для створення нових ресурсів та для редагування/оновлення існуючих.
 */
export const usePost = <TRequest, TResponse>(): UsePostResult<TResponse> => {
    const [data, setData] = useState<TResponse | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const post = async (url: string, data: TRequest, method: 'POST' | 'PUT' = 'POST') => {
        setLoading(true);
        setError(null);
        setData(null);

        const maxRetries = 3;
        for (let attempt = 0; attempt < maxRetries; attempt++) {
            try {
                const response = await fetch(url, {
                    method: method,
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(data),
                });

                if (!response.ok) {
                    let errorMessage = `HTTP Error: ${response.status} ${response.statusText}`;
                    try {
                        const errorBody = await response.json();
                        // Припускаємо, що API повертає об'єкт з деталями помилок
                        if (errorBody.errors) {
                            errorMessage += ": " + JSON.stringify(errorBody.errors);
                        } else if (errorBody.title) {
                            errorMessage += ": " + errorBody.title;
                        }
                    } catch {
                        // Ігноруємо помилку, якщо відповідь не JSON
                    }
                    throw new Error(errorMessage);
                }

                // API для POST/PUT може повертати 204 (No Content) або об'єкт.
                // Перевіряємо, чи є тіло відповіді
                if (response.headers.get('content-length') === '0' || response.status === 204) {
                    setData({} as TResponse); // Повертаємо порожній об'єкт у разі 204
                } else {
                    const result = await response.json();
                    setData(result);
                }
                
                break; // Успіх, виходимо з циклу
            } catch (err) {
                const message = err instanceof Error ? err.message : 'Невідома помилка POST/PUT';
                setError(message);
                
                if (attempt < maxRetries - 1) {
                    // Експоненційна затримка: 1с, 2с, 4с
                    const delay = Math.pow(2, attempt) * 1000;
                    await new Promise(resolve => setTimeout(resolve, delay));
                }
            } finally {
                setLoading(false);
            }
        }
    };

    return { data: data as TResponse | null, loading, error, post };
};