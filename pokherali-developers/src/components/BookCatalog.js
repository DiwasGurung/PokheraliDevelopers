
import { useState, useEffect } from "react"
import BookItem from "./BookItem"

// Sample book data
const allBooks = [
  {
    id: 1,
    title: "The Great Adventure",
    author: "John Smith",
    price: 24.99,
    rating: 4.5,
    genre: "Fiction",
    availability: "In stock",
    bestseller: true,
    newRelease: false,
    onSale: false,
  },
  {
    id: 2,
    title: "Mystery of the Blue Lake",
    author: "Sarah Johnson",
    price: 19.99,
    rating: 4.2,
    genre: "Mystery",
    availability: "In stock",
    bestseller: true,
    newRelease: true,
    onSale: false,
  },
  {
    id: 3,
    title: "The Science of Everything",
    author: "Robert Brown",
    price: 29.99,
    rating: 4.7,
    genre: "Science",
    availability: "In stock",
    bestseller: false,
    newRelease: false,
    onSale: true,
  },
  {
    id: 4,
    title: "Cooking Masterclass",
    author: "Maria Garcia",
    price: 34.99,
    rating: 4.8,
    genre: "Non-fiction",
    availability: "Out of stock",
    bestseller: false,
    newRelease: true,
    onSale: false,
  },
  {
    id: 5,
    title: "Fantasy World",
    author: "Michael Wilson",
    price: 22.99,
    rating: 4.3,
    genre: "Fantasy",
    availability: "In stock",
    bestseller: true,
    newRelease: false,
    onSale: true,
  },
  {
    id: 6,
    title: "Business Strategies",
    author: "Jennifer Lee",
    price: 27.99,
    rating: 4.0,
    genre: "Non-fiction",
    availability: "In stock",
    bestseller: false,
    newRelease: false,
    onSale: false,
  },
]

export default function BookCatalog({ filters = {} }) {
  const [filteredBooks, setFilteredBooks] = useState(allBooks)

  useEffect(() => {
    let result = allBooks

    // Apply genre filter
    if (filters.genre) {
      result = result.filter((book) => book.genre === filters.genre)
    }

    // Apply author filter
    if (filters.author) {
      result = result.filter((book) => book.author.toLowerCase().includes(filters.author.toLowerCase()))
    }

    // Apply price range filter
    if (filters.priceRange) {
      const [min, max] = filters.priceRange
      result = result.filter((book) => book.price >= min && book.price <= max)
    }

    // Apply rating filter
    if (filters.rating) {
      result = result.filter((book) => book.rating >= filters.rating)
    }

    // Apply language filter
    if (filters.language) {
      // In a real app, books would have a language property
      // This is just a placeholder
      result = result.filter((book) => book.language === filters.language)
    }

    // Apply format filter
    if (filters.format) {
      // In a real app, books would have a format property
      // This is just a placeholder
      result = result.filter((book) => book.format === filters.format)
    }

    // Apply publisher filter
    if (filters.publisher) {
      // In a real app, books would have a publisher property
      // This is just a placeholder
      result = result.filter((book) => book.publisher === filters.publisher)
    }

    // Apply availability filters
    if (filters.availability) {
      if (filters.availability.inStock) {
        result = result.filter((book) => book.availability === "In stock")
      }
      if (filters.availability.preOrder) {
        result = result.filter((book) => book.availability === "Pre-order")
      }
      // Library access would be another property in a real app
    }

    // Apply special category filters
    if (filters.specialCategories) {
      if (filters.specialCategories.bestseller) {
        result = result.filter((book) => book.bestseller)
      }
      if (filters.specialCategories.newRelease) {
        result = result.filter((book) => book.newRelease)
      }
      if (filters.specialCategories.onSale) {
        result = result.filter((book) => book.onSale)
      }
      // Other special categories would be handled similarly
    }

    setFilteredBooks(result)
  }, [filters])

  return (
    <div className="space-y-6">
      {filteredBooks.length === 0 ? (
        <div className="text-center py-12 bg-white rounded-xl shadow-sm border border-gray-100">
          <p className="text-gray-500">No books match your filters. Try adjusting your criteria.</p>
        </div>
      ) : (
        filteredBooks.map((book) => <BookItem key={book.id} book={book} />)
      )}
    </div>
  )
}
