import { useState } from 'react';

/**
 * Хук для виконання операцій видалення (DELETE-запитів) до API.
 * Повертає статус операції та функцію для виконання видалення.
 */
export function useDelete() {
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [isSuccess, setIsSuccess] = useState(false);

    const deleteData = async (url: string) => {
        setLoading(true);
        setError(null);
        setIsSuccess(false);

        try {
            const response = await fetch(url, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                },
            });

            if (response.ok) {
                setIsSuccess(true);
                console.log(`[useDelete] Успішне видалення: ${url}`);
            } else {
                const errorText = await response.text();
                throw new Error(`Помилка видалення (HTTP ${response.status}): ${errorText || response.statusText}`);
            }
        } catch (err) {
            const errorMessage = err instanceof Error ? err.message : "Невідома помилка видалення";
            setError(errorMessage);
            console.error(`[useDelete] Помилка видалення ${url}:`, err);
            setIsSuccess(false);
        } finally {
            setLoading(false);
        }
    };

    const resetStatus = () => {
        setLoading(false);
        setError(null);
        setIsSuccess(false);
    };

    return {
        deleteData,
        loading,
        error,
        isSuccess,
        resetStatus
    };
}