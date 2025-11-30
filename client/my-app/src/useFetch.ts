import { useState, useEffect } from 'react'
export function useFetch<T>(props: { url: string; })
{
    const [data, setData] = useState<T|null>(null);
    var myurl = props.url;
    useEffect(() => {
    fetch(myurl) // 1. Go to this URL
      .then(response => response.json())               // 2. Convert the response to JSON
      .then(json => {
        console.log(json);                             // 3. Log it so we can see it
        setData(json);                 // 4. Show it on screen (as text for now)
      })
      .catch(error => console.error("Error:", error)); // 5. Catch any network errors
  }, []);
  return{
        data
    };
}
