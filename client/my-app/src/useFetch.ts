import { useState, useEffect, useCallback } from 'react';

/**
 * Хук для отримання даних з API з підтримкою завантаження, помилок та ручного оновлення.
 * @param url URL для запиту
 * @param skip Чи пропускати запит (корисно для деталей, де ID може бути null)
 */
export function useFetch<T>({ url, skip = false }: { url: string; skip?: boolean }) {
    const [data, setData] = useState<T | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [fetchKey, setFetchKey] = useState(0); // Ключ для примусового оновлення

    // Функція для примусового оновлення даних
    const reFetch = useCallback(() => {
        setFetchKey(prevKey => prevKey + 1);
    }, []);

    useEffect(() => {
        if (skip || !url) {
            setData(null);
            return;
        }

        const fetchData = async () => {
            setLoading(true);
            setError(null);
            
            try {
                const response = await fetch(url);
                if (!response.ok) {
                    throw new Error(`Помилка HTTP: ${response.statusText}`);
                }
                const json: T = await response.json();
                
                console.log(`[useFetch] Отримані дані з ${url}:`, json); 
                
                setData(json);
            } catch (err) {
                const errorMessage = err instanceof Error ? err.message : "Невідома помилка під час отримання даних";
                setError(errorMessage);
                console.error(`[useFetch] Помилка отримання даних з ${url}:`, err);
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [url, skip, fetchKey]);

    return {
        data,
        loading,
        error,
        reFetch
    };
}