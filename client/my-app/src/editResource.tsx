import { useParams, useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";
import { usePost } from "./usePost";
import { useFetch } from "./useFetch";
import { CultureType, getCultureName } from "./App";
import type { Resource as ResourceType } from "./App";


const detailsUrlTemplate = "https://localhost:7289/api/v1/Resources";

interface ResourceEditRequest extends ResourceType {
    yieldValue: number; // API очікує yieldValue, а не yield для POST
}

export function EditResource() {
    const { id } = useParams<{ id: string }>();
    const resourceId = id ? parseInt(id) : null;
    const navigate = useNavigate();

    // 1. Завантаження поточних даних
    const detailsUrl = `${detailsUrlTemplate}/${resourceId}`;
    const { data: initialResource, loading: fetchLoading, error: fetchError } = useFetch<ResourceType>({ url: detailsUrl, skip: resourceId === null });

    // 2. Логіка POST для оновлення
    const { data: updatedResource, error: updateError, loading: updateLoading, post } = usePost<ResourceEditRequest, ResourceType>();

    // Локальний стан форми
    const [seedPerHectare, setSeedPerHectare] = useState(0);
    const [cultureType, setCulture] = useState(0);
    const [fertilizerPerHectare, setFertilizerPerHectare] = useState(0);
    const [workerPerHectare, setWorkerPerHectare] = useState(0);
    const [workerWorkDuralityPerHectare, setWorkerWorkDuralityPerHectare] = useState(0);
    const [yieldValue, setYieldValue] = useState(0); // Використовуємо yieldValue для форми

    // 3. Заповнення форми, коли дані завантажені
    useEffect(() => {
        if (initialResource) {
            setSeedPerHectare(initialResource.seedPerHectare);
            setCulture(initialResource.cultureType);
            setFertilizerPerHectare(initialResource.fertilizerPerHectare);
            setWorkerPerHectare(initialResource.workerPerHectare);
            setWorkerWorkDuralityPerHectare(initialResource.workerWorkDuralityPerHectare);
            setYieldValue(initialResource.yield); // Ініціалізуємо з поля yield
        }
    }, [initialResource]);

    // Перенаправлення після успішного оновлення
    useEffect(() => {
        if (updatedResource && !updateError) {
            navigate(`/resources/${resourceId}`);
        }
    }, [updatedResource, updateError, navigate, resourceId]);

    const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!initialResource || resourceId === null) return;

    // Тіло PUT-запиту повинно відповідати ResourceUpdateDto
    const updatedData = {
        cultureType, // співпадає з DTO
        seedPerHectare,
        fertilizerPerHectare,
        workerPerHectare,
        workerWorkDuralityPerHectare,
        yield: yieldValue, // тепер поле yield, як чекає бекенд
    };

    try {
        // Відправляємо PUT на /Resources/{id}
        await post(`${detailsUrlTemplate}/${resourceId}`, updatedData, 'PUT');
    } catch (err) {
        console.error('Error updating resource:', err);
    }
};

    if (resourceId === null) return <p className="text-red-600 p-4">Недійсний ID ресурсу.</p>;
    if (fetchLoading) return <p className="text-lg text-center mt-10">Завантаження даних ресурсу...</p>;
    if (fetchError) return <p className="text-red-600 p-4">Помилка завантаження: {fetchError}</p>;
    if (!initialResource) return <p className="text-red-600 p-4">Ресурс з ID {resourceId} не знайдено.</p>;

    return (
        <div className="max-w-md mx-auto mt-10 p-8 bg-white shadow-2xl rounded-xl border border-yellow-200">
            <h2 className="text-2xl font-extrabold mb-6 text-center text-yellow-700">
                Редагувати Ресурс для {getCultureName(initialResource.cultureType)} (ID: {resourceId})
            </h2>

            <form onSubmit={handleSubmit} className="space-y-4">
                {/* ... (Всі поля вводу, аналогічні CreateResource, з використанням локального стану) */}
                
                {/* CULTURE */}
                <div>
                    <label className="block font-semibold mb-1 text-gray-700">Культура</label>
                    <select
                        value={cultureType}
                        onChange={(e) => setCulture(parseInt(e.target.value))}
                        className="border border-gray-300 rounded-lg w-full px-4 py-2 focus:ring-yellow-500 focus:border-yellow-500 transition duration-150 bg-white"
                        required
                    >
                        {Object.keys(CultureType)
                            .filter(key => isNaN(Number(key)))
                            .map((key) => (
                                <option key={key} value={CultureType[key as keyof typeof CultureType]}>
                                    {key}
                                </option>
                            ))}
                    </select>
                </div>

                {/* SeedPerHectare */}
                <div>
                    <label className="block font-semibold mb-1 text-gray-700">Насіння на гектар</label>
                    <input
                        type="number"
                        step="0.1"
                        min="0"
                        value={seedPerHectare}
                        onChange={(e) => setSeedPerHectare(parseFloat(e.target.value))}
                        className="border border-gray-300 rounded-lg w-full px-4 py-2"
                        required
                    />
                </div>

                {/* FertilizerPerHectare */}
                <div>
                    <label className="block font-semibold mb-1 text-gray-700">Добрива на гектар</label>
                    <input
                        type="number"
                        step="0.1"
                        min="0"
                        value={fertilizerPerHectare}
                        onChange={(e) => setFertilizerPerHectare(parseFloat(e.target.value))}
                        className="border border-gray-300 rounded-lg w-full px-4 py-2"
                        required
                    />
                </div>

                {/* WorkerPerHectare */}
                <div>
                    <label className="block font-semibold mb-1 text-gray-700">Робітників на гектар</label>
                    <input
                        type="number"
                        step="0.01"
                        min="0"
                        value={workerPerHectare}
                        onChange={(e) => setWorkerPerHectare(parseFloat(e.target.value))}
                        className="border border-gray-300 rounded-lg w-full px-4 py-2"
                        required
                    />
                </div>

                {/* WorkerWorkDuralityPerHectare */}
                <div>
                    <label className="block font-semibold mb-1 text-gray-700">Тривалість робіт/га (години)</label>
                    <input
                        type="number"
                        step="0.1"
                        min="0"
                        value={workerWorkDuralityPerHectare}
                        onChange={(e) => setWorkerWorkDuralityPerHectare(parseFloat(e.target.value))}
                        className="border border-gray-300 rounded-lg w-full px-4 py-2"
                        required
                    />
                </div>
                
                {/* YieldValue */}
                <div>
                    <label className="block font-semibold mb-1 text-gray-700">Очікувана врожайність</label>
                    <input
                        type="number"
                        step="1"
                        min="0"
                        value={yieldValue}
                        onChange={(e) => setYieldValue(parseFloat(e.target.value))}
                        className="border border-gray-300 rounded-lg w-full px-4 py-2"
                        required
                    />
                </div>

                <button
                    type="submit"
                    disabled={updateLoading}
                    className="w-full bg-yellow-600 text-white font-bold py-3 rounded-lg hover:bg-yellow-700 transition duration-200 disabled:opacity-50 shadow-md hover:shadow-lg"
                >
                    {updateLoading ? "Оновлення..." : "Зберегти Зміни"}
                </button>

            </form>

            {updateError && <p className="text-red-600 mt-4 p-2 bg-red-100 rounded">Помилка оновлення: {updateError}</p>}
        </div>
    );
}