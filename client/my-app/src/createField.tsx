import { usePost } from "./usePost"; // твій хук
import { useState } from "react";

const createUrl = "https://localhost:7289/api/v1/Field/create";

interface FieldCreateRequest {
  area: number;
  culture: number;
  status: number;
  workers: any[];
  machines: any[];
  tasks: any[];
}

interface Field {
  id: number;
  area: number;
  culture: number;
  status: number;
  createdAt: string;
}

export function CreateField() {
  const { data, error, loading, post } = usePost<FieldCreateRequest, Field>();

  const [area, setArea] = useState(0);
  const [culture, setCulture] = useState(0);
  const [status, setStatus] = useState(0);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();

    const newField: FieldCreateRequest = {
      area,
      culture,
      status,
      workers: [],
      machines: [],
      tasks: []
    };

    await post(createUrl, newField);
  }

  return (
    <div className="max-w-md mx-auto mt-10 p-6 bg-gray-100 shadow-md rounded-xl">
      <h2 className="text-xl font-bold mb-4 text-center">Create New Field</h2>

      <form onSubmit={handleSubmit} className="space-y-4">

        <div>
          <label className="block font-medium mb-1">Area</label>
          <input
            type="number"
            value={area}
            onChange={(e) => setArea(parseInt(e.target.value))}
            className="border rounded w-full px-3 py-2"
          />
        </div>

        <div>
          <label className="block font-medium mb-1">Culture</label>
          <input
            type="number"
            value={culture}
            onChange={(e) => setCulture(parseInt(e.target.value))}
            className="border rounded w-full px-3 py-2"
          />
        </div>

        <div>
          <label className="block font-medium mb-1">Status</label>
          <input
            type="number"
            value={status}
            onChange={(e) => setStatus(parseInt(e.target.value))}
            className="border rounded w-full px-3 py-2"
          />
        </div>

        <button
          type="submit"
          disabled={loading}
          className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700"
        >
          {loading ? "Creating..." : "Create Field"}
        </button>

      </form>

      {error && <p className="text-red-600 mt-3">Error: {error}</p>}
      {data && (
        <p className="text-green-600 mt-3">
          ✅ Field created successfully! ID: {data.id}
        </p>
      )}
    </div>
  );
}
