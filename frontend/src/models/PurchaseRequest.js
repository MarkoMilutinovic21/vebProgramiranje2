export const createPurchaseRequest = (userId, bookTitle, author, quantity, email) => {
    return {
        userId,
        bookTitle,
        author,
        quantity,
        email
    };
};