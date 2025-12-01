import { usePost } from "./usePost";
import { useState } from "react";
import { CultureType} from "./App";
import { useNavigate } from "react-router-dom";
import type { Resource as ResourceType } from "./App";
const createUrl = "https://localhost:7289/api/v1/Resources";

interface ResourceCreateRequest {
    cultureType: number;
    seedPerHectare: number;
    fertilizerPerHectare: number;
    workerPerHectare: number;
    workerWorkDuralityPerHectare: number;
    yieldValue: number; // Назва поля, яку очікує API
    requiredMachines: any[];
}

export function CreateResource() {
    const { data, error, loading, post } = usePost<ResourceCreateRequest, ResourceType>();
    const navigate = useNavigate();

    const [seedPerHectare, setSeedPerHectare] = useState(5);
    const [cultureType, setCulture] = useState(0);
    const [fertilizerPerHectare, setFertilizerPerHectare] = useState(2);
    const [workerPerHectare, setWorkerPerHectare] = useState(0.1);
    const [workerWorkDuralityPerHectare, setWorkerWorkDuralityPerHectare] = useState(2);
    const [yieldValue, setYieldValue] = useState(30);

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();

        const newResource: ResourceCreateRequest = {
            seedPerHectare,
            cultureType,
            fertilizerPerHectare,
            workerPerHectare,
            workerWorkDuralityPerHectare,
            yieldValue,
            requiredMachines: []
        };

        await post(createUrl, newResource);

        // Перенаправлення після успішного створення
        if (!error && data) {
            navigate("/resources");
        }
    }

    return (
        <div className="max-w-md mx-auto mt-10 p-8 bg-white shadow-2xl rounded-xl border border-green-200">
            <h2 className="text-2xl font-extrabold mb-6 text-center text-green-700">Створити Новий Ресурс</h2>

            <form onSubmit={handleSubmit} className="space-y-4">

                <div>
                    <label className="block font-semibold mb-1 text-gray-700">Культура</label>
                    <select
                        
                        value={cultureType}
                        onChange={(e) => setCulture(parseInt(e.target.value))}
                        className="border border-gray-300 rounded-lg w-full px-4 py-2 focus:ring-green-500 focus:border-green-500 transition duration-150 bg-white"
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
                    disabled={loading}
                    className="w-full bg-green-600 text-white font-bold py-3 rounded-lg hover:bg-green-700 transition duration-200 disabled:opacity-50 shadow-md hover:shadow-lg"
                >
                    {loading ? "Створення..." : "Створити Ресурс"}
                </button>

            </form>

            {error && <p className="text-red-600 mt-4 p-2 bg-red-100 rounded">Помилка: {error}</p>}
            {data && (
                <p className="text-green-600 mt-4 p-2 bg-green-100 rounded">
                    ✅ Ресурс успішно створено! ID: {data.id}
                </p>
            )}
        </div>
    );
}