import { useState, useEffect } from 'react'
import { Routes, Route, Link, Navigate } from 'react-router-dom';
import { useParams } from "react-router-dom";
import './App.css'
import {useFetch} from './useFetch.ts'
import { CreateField } from './createField.tsx';
import { CreateResource } from './createResource.tsx';
import { useDelete } from './useDelete.ts'
import { EditField } from './editField.tsx';
import { EditResource } from './editResource.tsx';
interface Worker{
  id:number;
  firstName: string;
  lastName: string;
  age: number;
  hourlyRate: number;
  isActive: boolean;
  hoursWorked: number;
  tasks: Task[];
}
interface Machine{
  id:number;
  type: number;
  fuelConsumption:number;
  isAvailable: boolean;
  workDuralityPerHectare:number;
  fieldId:number;
  resourceId:number;
}
interface Task{
  id:number;
  description:string;
  workerId:number;
  fieldId:number;
  type: number;
  progress:number;
  startDate:string;
  realEndDate:string;
  estimatedEndDate:string;
}
export const TaskType = 
{
    Planting:0,
    Harvesting:1,
    Irrigation:2,
    Fertilization:3,
    PestControl:4
} as const;
export const MachineType=
{
    Tractor:0,
    Harvester:1,
    Plow:2,
    Seeder:3,
    Sprayer:4
} as const;
export const CultureType = 
{
    Wheat:0,
    Corn:1,
    Soybean:2,
    Rice:3,
    Cotton:4
} as const;
export const FieldStatus =
{
    Planted: 0,
    Harvested: 1,
    Fallow:2
} as const;
interface Field{
  id: number;
  area: number;
  culture: number;
  status:number;
  workers: Worker[];
  machines: Machine[];
  tasks: Task[];
  createdAt: string;
}
interface Resource{
  id:number;
  cultureType: number;
  seedPerHectare:number;
  fertilizerPerHectare:number;
  workerPerHectare: number;
  workerWorkDuralityPerHectare:number;
  yield:number;
  requiredMachines: Machine[];
}
export function getCultureName(value: number): string {
    const keys = Object.keys(CultureType).filter(k => typeof CultureType[k as keyof typeof CultureType] === 'number');
    const cultureKey = keys.find(key => CultureType[key as keyof typeof CultureType] === value);
    return cultureKey || "Невідома культура";
}

// Функція для отримання текстової назви Status
export function getStatusName(value: number): string {
    const keys = Object.keys(FieldStatus).filter(k => typeof FieldStatus[k as keyof typeof FieldStatus] === 'number');
    const statusKey = keys.find(key => FieldStatus[key as keyof typeof FieldStatus] === value);
    return statusKey || "Невідомий статус";
}
export type { Field , Resource};
function App() {
  return (
    <>
      {/* Додано компонент навігації для переходу між основними розділами */}
      <nav className="bg-gray-800 p-4">
        <ul className="flex space-x-4">
          <li><Link to="/fields" className="text-white hover:text-green-400 transition">Поля</Link></li>
          <li><Link to="/resources" className="text-white hover:text-green-400 transition">Ресурси</Link></li>
        </ul>
      </nav>
      <Routes>
        {/* ВИПРАВЛЕНО: Додано маршрут перенаправлення з кореневого шляху "/" */}
        <Route path="/" element={<Navigate to="/fields" replace />} />
        <Route path="/fields" element={<IndexFieldEndPoint />}/>
        <Route path="/fields/create" element={<CreateField />} />
        <Route path="/fields/edit/:id" element={<EditField />} /> 
        <Route path="/fields/details/:id" element={<DetailsFieldEndPoint />} />
        <Route path="/resources/edit/:id" element={<EditResource />} /> 
        <Route path="/resources" element={<IndexResourceEndPoint />}/>
        <Route path="/resources/create" element={<CreateResource/>} />
        <Route path="/resources/:id" element={<DetailsResourceEndPoint />} />
      </Routes>
    </>
  )
}
// У App.tsx
// ВИПРАВЛЕНО: Розширено інлайн-тип, щоб включити onDelete та deleteLoading
function FieldCard({ field, onDelete, deleteLoading }: { 
    field: Field;
    onDelete: (id: number) => Promise<void>;
    deleteLoading: boolean;
}) {
  // Використовуємо функції-маппери
  const cultureName = getCultureName(field.culture); 
  const statusName = getStatusName(field.status);

  return (
    <div className="field-card border p-4 mb-3 rounded shadow-sm hover:bg-gray-50">
      <h3 className="text-lg font-bold">Поле #{field.id}</h3>
      <p>
        <span className="font-semibold">Площа:</span> {field.area} га | 
        <span className="font-semibold">Культура:</span> <span className="font-bold text-green-600">{cultureName}</span> |
        <span className="font-semibold">Статус:</span> <span className="italic text-blue-600">{statusName}</span>
      </p>
      <div className="flex space-x-2 mt-2">
        {/* Використовуємо Link для Деталей */}
        <Link to={`/fields/details/${field.id}`} className="text-blue-600 hover:underline">Деталі</Link>
        {/* Використовуємо Link для Редагування */}
        <Link to={`/fields/edit/${field.id}`} className="text-yellow-600 hover:underline">Редагувати</Link>
        <button 
            onClick={() => onDelete(field.id)}
            disabled={deleteLoading}
            className="text-red-600 hover:underline disabled:opacity-50"
        >
            {deleteLoading ? "Видалення..." : "Видалити"}
        </button>
      </div>
    </div>
  );
}

function IndexFieldEndPoint() {
    const url = "https://localhost:7289/api/v1/Field/index";
    // 1. Отримуємо всі необхідні стани з useFetch
    const { data: fetchedFields, loading, error, reFetch } = useFetch<Field[]>({ url });
    // 2. Ініціалізуємо useDelete
    const { deleteData, loading: deleteLoading, error: deleteError, isSuccess, resetStatus } = useDelete();
    const [lastDeletedId, setLastDeletedId] = useState<number | null>(null);

    // 3. Функція видалення
    const handleDelete = async (fieldId: number) => {
        resetStatus(); // Скидаємо статус перед новою спробою
        if (window.confirm(`Ви впевнені, що хочете видалити поле #${fieldId}?`)) {
            const deleteUrl = `https://localhost:7289/api/v1/Field/delete/${fieldId}`;
            setLastDeletedId(fieldId);
            await deleteData(deleteUrl);
        }
    };

    // 4. Оновлюємо список полів після успішного видалення
    useEffect(() => {
        if (isSuccess && lastDeletedId !== null) {
            reFetch();
            setLastDeletedId(null);
        }
    }, [isSuccess, reFetch, lastDeletedId]); 

    // 5. Обробка станів завантаження/помилок
    if (loading) return <div className="text-center mt-10 text-xl font-medium">Завантаження полів...</div>;
    if (error) return <div className="text-red-600 p-4 border border-red-300 bg-red-50 rounded">Помилка завантаження: {error}</div>;


    return (
        <div className="p-6">
            <h1 className="text-3xl font-bold mb-6 text-gray-800">Управління Полями</h1>
            <Link to="/fields/create" className="inline-block bg-green-500 text-white px-4 py-2 rounded-lg hover:bg-green-600 transition duration-200 mb-6 shadow-md">
                + Створити Нове Поле
            </Link>

            {deleteError && <div className="text-red-600 p-3 bg-red-100 rounded mb-4">Помилка видалення: {deleteError}</div>}
            
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {fetchedFields && fetchedFields.length > 0 ? (
                    fetchedFields.map((field) => (
                        <FieldCard 
                            key={field.id} 
                            field={field} 
                            // Передаємо функцію видалення та статус завантаження
                            onDelete={handleDelete} 
                            deleteLoading={deleteLoading && lastDeletedId === field.id}
                        />
                    ))
                ) : (
                    <p className="text-gray-500 col-span-full">Список полів порожній.</p>
                )}
            </div>
            
        </div>
    );
}
function DetailsFieldEndPoint() {
  const { id } = useParams();
  const url = `https://localhost:7289/api/v1/Field/details/${id}`;

  const { data: fetchedField } = useFetch<Field>({ url });

  if (!fetchedField) return <p>Loading...</p>;

  return <FieldCard 
    field={fetchedField} 
    // ВИПРАВЛЕНО: Використовуємо async () => {} для відповідності типу Promise<void>
    onDelete={async () => { console.log('Delete handler disabled on details page.'); }} 
    deleteLoading={false} 
  />;
}

// У App.tsx
// ВИПРАВЛЕНО: Розширено інлайн-тип, щоб включити onDelete та deleteLoading
function ResourceCard({ resource, onDelete, deleteLoading }: { 
    resource: Resource; 
    onDelete: (id: number) => Promise<void>;
    deleteLoading: boolean;
}) {
    const cultureName = getCultureName(resource.cultureType);

    return (
        <div className="resource-card border p-4 mb-3 rounded shadow-sm hover:bg-gray-50">
            <h3 className="text-lg font-bold">Ресурс для {cultureName} (ID: {resource.id})</h3>
            <p>
                **Насіння/га:** {resource.seedPerHectare} | 
                **Добрива/га:** {resource.fertilizerPerHectare} |
                **Робітники/га:** {resource.workerPerHectare} |
                **Тривалість робіт/га:** {resource.workerWorkDuralityPerHectare} год |
                **Врожайність (оцінка):** {resource.yield}
            </p>
            <div className="flex space-x-2 mt-2">
                <a href={`/resources/${resource.id}`} className="text-blue-600 hover:underline">Деталі</a>
                <Link to={`/resources/edit/${resource.id}`} className="text-yellow-600 hover:underline">Редагувати</Link>
                
                <button 
                    onClick={() => onDelete(resource.id)}
                    disabled={deleteLoading}
                    className="text-red-600 hover:underline disabled:opacity-50"
                >
                    {deleteLoading ? "Видалення...": "Видалити"}
                </button>
            </div>
        </div>
    );
}
function IndexResourceEndPoint() {
    const url = "https://localhost:7289/api/v1/Resources";
    // 1. Отримуємо всі необхідні стани з useFetch
    const { data: fetchedResources, loading, error, reFetch } = useFetch<Resource[]>({ url });
    // 2. Ініціалізуємо useDelete
    const { deleteData, loading: deleteLoading, error: deleteError, isSuccess, resetStatus } = useDelete();
    const [lastDeletedId, setLastDeletedId] = useState<number | null>(null);
    
    // 3. Функція видалення
    const handleDelete = async (resourceId: number) => {
        resetStatus();
        if (window.confirm(`Ви впевнені, що хочете видалити ресурс #${resourceId}?`)) {
            const deleteUrl = `https://localhost:7289/api/v1/Resources/${resourceId}`;
            setLastDeletedId(resourceId);
            await deleteData(deleteUrl);
        }
    };

    // 4. Оновлюємо список ресурсів після успішного видалення
    useEffect(() => {
        if (isSuccess && lastDeletedId !== null) {
            reFetch();
            setLastDeletedId(null);
        }
    }, [isSuccess, reFetch, lastDeletedId]); 

    // 5. Обробка станів завантаження/помилок
    if (loading) return <div className="text-center mt-10 text-xl font-medium">Завантаження ресурсів...</div>;
    if (error) return <div className="text-red-600 p-4 border border-red-300 bg-red-50 rounded">Помилка завантаження: {error}</div>;

    return (
        <div className="p-6">
            <h1 className="text-3xl font-bold mb-6 text-gray-800">Управління Ресурсами</h1>
            <Link to="/resources/create" className="inline-block bg-green-500 text-white px-4 py-2 rounded-lg hover:bg-green-600 transition duration-200 mb-6 shadow-md">
                + Створити Новий Ресурс
            </Link>

            {deleteError && <div className="text-red-600 p-3 bg-red-100 rounded mb-4">Помилка видалення: {deleteError}</div>}
            
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {fetchedResources && fetchedResources.length > 0 ? (
                    fetchedResources.map((resource) => (
                        <ResourceCard 
                            key={resource.id} 
                            resource={resource} 
                            // Передаємо функцію видалення та статус завантаження
                            onDelete={handleDelete} 
                            deleteLoading={deleteLoading && lastDeletedId === resource.id}
                        />
                    ))
                ) : (
                    <p className="text-gray-500 col-span-full">Список ресурсів порожній.</p>
                )}
            </div>
        </div>
    );
}
 function DetailsResourceEndPoint() {
  const { id } = useParams();
  const url = `https://localhost:7289/api/v1/Resources/${id}`;

  const { data: fetchedResource } = useFetch<Resource>({ url });

  if (!fetchedResource) return <p>Loading...</p>;

  return <ResourceCard 
    resource={fetchedResource} 
    
    onDelete={async () => { console.log('Delete handler disabled on details page.'); }} 
    deleteLoading={false} 
  />;
}
export default App