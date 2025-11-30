import { useState } from 'react'
import { Routes, Route } from 'react-router-dom';
import { useParams } from "react-router-dom";
import './App.css'
import {useFetch} from './useFetch.ts'
import { CreateField } from './createField.tsx';
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
  culture: number;
  seedPerHectare:number;
  fertilizerPerHectare:number;
  workerPerHectare: number;
  workerWorkDuralityPerHectare:number;
  yield:number;
  requiredMachines: Machine[];
}
function App() {
  return (
    <>
      <Routes>
        <Route path="/fields" element={<IndexFieldEndPoint />}/>
        <Route path="/fields/create" element={<CreateField />} />
        <Route path="/fields/details/:id" element={<DetailsFieldEndPoint />} />
      </Routes>
    </>
  )
}
function FieldCard({ field }: { field: Field }) {
  return (
    <p>
      Field id: {field.id} | Area: {field.area} | Culture: {field.culture} |
      Status: {field.status}
    </p>
  );
}

function IndexFieldEndPoint() {

    const url = "https://localhost:7289/api/v1/Field/index";
    const {data: fetchedFields} = useFetch<Field[]>({url});
    
    return (
        <div>
          {(fetchedFields?.map((field) => (
            <FieldCard key={field.id} field={field} />
          )))}
        </div>
      );
 }
function DetailsFieldEndPoint() {
  const { id } = useParams();
  const url = `https://localhost:7289/api/v1/Field/details/${id}`;

  const { data: fetchedField } = useFetch<Field>({ url });

  if (!fetchedField) return <p>Loading...</p>;

  return <FieldCard field={fetchedField} />;
}

export default App
