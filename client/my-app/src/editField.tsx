import { useParams, useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";
import { usePost } from "./usePost"; // Використовуємо usePost для PUT/POST оновлення
import { useFetch } from "./useFetch";
// ВИПРАВЛЕНО: Імпортуємо Field як тип за замовчуванням (Default Export) під аліасом FieldType,
// оскільки Field, ймовірно, не експортований іменовано або конфліктує з експортом за замовчуванням.
import { CultureType, FieldStatus, getCultureName, getStatusName } from "./App"; 
import type {Field as FieldType } from "./App"; 

const editUrlTemplate = "https://localhost:7289/api/v1/Field/edit";
const detailsUrlTemplate = "https://localhost:7289/api/v1/Field/details";


export function EditField() {
    const { id } = useParams<{ id: string }>();
    const fieldId = id ? parseInt(id) : null;
    const navigate = useNavigate();

    // 1. Завантаження поточних даних
    const detailsUrl = `${detailsUrlTemplate}/${fieldId}`;
    const { data: initialField, loading: fetchLoading, error: fetchError } = useFetch<FieldType>({ url: detailsUrl, skip: fieldId === null }); // ВИКОРИСТАННЯ FieldType

    // 2. Логіка POST для оновлення
    const { data: updatedField, error: updateError, loading: updateLoading, post } = usePost<FieldType, FieldType>(); // ВИКОРИСТАННЯ FieldType

    const [area, setArea] = useState(0);
    const [culture, setCulture] = useState(0);
    const [status, setStatus] = useState(0);

    // 3. Заповнення форми, коли дані завантажені
    useEffect(() => {
        if (initialField) {
            setArea(initialField.area);
            setCulture(initialField.culture);
            setStatus(initialField.status);
        }
    }, [initialField]);

    // Перенаправлення після успішного оновлення
    useEffect(() => {
        if (updatedField && !updateError) {
            navigate(`/fields/details/${fieldId}`);
        }
    }, [updatedField, updateError, navigate, fieldId]);


    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!initialField || fieldId === null) return;

        const updatedData: FieldType = { // ВИКОРИСТАННЯ FieldType
            ...initialField, // Зберігаємо всі інші дані
            area,
            culture,
            status,
        };

        // Відправка POST запиту на ендпоінт /edit/{id}
        await post(`${editUrlTemplate}/${fieldId}`, updatedData, 'POST');
    };

    if (fieldId === null) return <p className="text-red-600 p-4">Недійсний ID поля.</p>;
    if (fetchLoading) return <p className="text-lg text-center mt-10">Завантаження даних поля...</p>;
    if (fetchError) return <p className="text-red-600 p-4">Помилка завантаження: {fetchError}</p>;
    if (!initialField) return <p className="text-red-600 p-4">Поле з ID {fieldId} не знайдено.</p>;

    return (
        <div className="max-w-md mx-auto mt-10 p-8 bg-white shadow-2xl rounded-xl border border-yellow-200">
            <h2 className="text-2xl font-extrabold mb-6 text-center text-yellow-700">
                Редагувати Поле #{fieldId} ({getCultureName(initialField.culture)})
            </h2>

            <form onSubmit={handleSubmit} className="space-y-4">
                {/* AREA */}
                <div>
                    <label className="block font-semibold mb-1 text-gray-700">Площа (га)</label>
                    <input
                        type="number"
                        min="0"
                        value={area}
                        onChange={(e) => setArea(parseFloat(e.target.value))}
                        className="border border-gray-300 rounded-lg w-full px-4 py-2 focus:ring-yellow-500 focus:border-yellow-500 transition duration-150"
                        required
                    />
                </div>

                {/* CULTURE */}
                <div>
                    <label className="block font-semibold mb-1 text-gray-700">Культура</label>
                    <select
                        value={culture}
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

                {/* STATUS */}
                <div>
                    <label className="block font-semibold mb-1 text-gray-700">Статус</label>
                    <select
                        value={status}
                        onChange={(e) => setStatus(parseInt(e.target.value))}
                        className="border border-gray-300 rounded-lg w-full px-4 py-2 focus:ring-yellow-500 focus:border-yellow-500 transition duration-150 bg-white"
                        required
                    >
                        {Object.keys(FieldStatus)
                            .filter(key => isNaN(Number(key)))
                            .map((key) => (
                                <option key={key} value={FieldStatus[key as keyof typeof FieldStatus]}>
                                    {key}
                                </option>
                            ))}
                    </select>
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