import { useState } from "react";
import { purchaseBook } from "../services/purchaseService";

const users = [
    { id: "user1", name: "John Doe", email: "john@example.com" },
    { id: "user2", name: "Jane Doe", email: "jane@example.com" }
];

function PurchaseForm() {
    const [formData, setFormData] = useState({
        userId: "user1",
        bookTitle: "",
        author: "",
        quantity: 1
    });

    const [message, setMessage] = useState("");
    const [error, setError] = useState("");

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setMessage("");
        setError("");

        const selectedUser = users.find(u => u.id === formData.userId);

        const purchaseRequest = {
            userId: formData.userId,
            bookId: "book1",
            quantity: parseInt(formData.quantity),
            email: selectedUser.email
        };

        try {
            const result = await purchaseBook(purchaseRequest);
            setMessage(result);
        } catch (err) {
            setError(err.message);
        }
    };

    return (
        <div>
            <h2>Book Purchase</h2>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Book Title</label>
                    <input
                        type="text"
                        name="bookTitle"
                        value={formData.bookTitle}
                        onChange={handleChange}
                        required
                    />
                </div>
                <div>
                    <label>Author</label>
                    <input
                        type="text"
                        name="author"
                        value={formData.author}
                        onChange={handleChange}
                        required
                    />
                </div>
                <div>
                    <label>Quantity</label>
                    <input
                        type="number"
                        name="quantity"
                        value={formData.quantity}
                        onChange={handleChange}
                        min="1"
                        required
                    />
                </div>
                <div>
                    <label>User Account</label>
                    <select name="userId" value={formData.userId} onChange={handleChange}>
                        {users.map(user => (
                            <option key={user.id} value={user.id}>
                                {user.name} - {user.email}
                            </option>
                        ))}
                    </select>
                </div>
                <button type="submit">Confirm Purchase</button>
            </form>
            {message && <p style={{ color: "green" }}>{message}</p>}
            {error && <p style={{ color: "red" }}>{error}</p>}
        </div>
    );
}

export default PurchaseForm;