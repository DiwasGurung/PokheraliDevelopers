import React, { useState } from "react"
import { Search } from 'lucide-react'

function SearchBar() {
  const [query, setQuery] = useState("")

  const handleSearch = (e) => {
    e.preventDefault()
    // Implement search functionality
    console.log("Searching for:", query)
  }

  return (
    <form onSubmit={handleSearch} className="relative w-full max-w-md">
      <input
        type="text"
        value={query}
        onChange={(e) => setQuery(e.target.value)}
        placeholder="Search by title, author, or ISBN..."
        className="w-full py-2 pl-4 pr-10 rounded-full border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
      />
      <button
        type="submit"
        className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 hover:text-blue-600"
      >
        <Search size={18} />
      </button>
    </form>
  )
}

export default function Header() {
  return (
    <header className="bg-white shadow-md">
      <div className="container mx-auto px-4 py-4">
        <div className="flex flex-col md:flex-row items-center justify-between gap-4">
          <a href="/" className="text-2xl font-bold text-blue-600">
            BookStore
          </a>

          <nav className="w-full md:w-auto">
            <ul className="flex flex-wrap justify-center gap-6">
              <li>
                <a href="/" className="text-gray-700 hover:text-blue-600 font-medium">
                  All Books
                </a>
              </li>
              <li>
                <a href="/bestsellers" className="text-gray-700 hover:text-blue-600 font-medium">
                  Bestsellers
                </a>
              </li>
              <li>
                <a href="/new-releases" className="text-gray-700 hover:text-blue-600 font-medium">
                  New Releases
                </a>
              </li>
              <li>
                <a href="/deals" className="text-gray-700 hover:text-blue-600 font-medium">
                  Deals
                </a>
              </li>
              <li>
                <a href="/login" className="text-gray-700 hover:text-blue-600 font-medium">
                  Login
                                  </a>
              </li>
            </ul>
          </nav>

          <SearchBar />
        </div>
      </div>
    </header>
  )
}
