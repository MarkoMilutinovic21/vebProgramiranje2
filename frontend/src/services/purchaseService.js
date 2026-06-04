const API_URL = import.meta.env.VITE_API_URL;

export const purchaseBook = async (purchaseRequest) => {
    const response = await fetch(`${API_URL}/api/purchase`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(purchaseRequest)
    });

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error);
    }

    return await response.text();
};