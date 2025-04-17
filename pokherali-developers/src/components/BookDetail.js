import { Star, ShoppingCart, Bookmark, Heart, Share2, ChevronLeft, Award, Clock } from "lucide-react"

export default function BookDetail({ book }) {
  const renderStars = (rating) => {
    return Array(5)
      .fill(0)
      .map((_, i) => (
        <Star
          key={i}
          size={18}
          className={i < Math.floor(rating) ? "text-yellow-400 fill-yellow-400" : "text-gray-300"}
        />
      ))
  }

  // Calculate half stars for more accurate ratings
  const hasHalfStar = book?.rating % 1 >= 0.5

  return (
    <div className="max-w-7xl mx-auto bg-white rounded-xl shadow-md overflow-hidden">
      {/* Breadcrumb navigation */}
      <div className="px-6 pt-6 pb-2">
        <a
          href="#"
          className="inline-flex items-center text-sm text-purple-600 hover:text-purple-800 transition-colors"
        >
          <ChevronLeft size={16} className="mr-1" />
          Back to Books
        </a>
      </div>

      <div className="p-6">
        <div className="flex flex-col lg:flex-row gap-8">
          {/* Book image with badges */}
          <div className="lg:w-1/3 relative">
            <div className="aspect-[3/4] rounded-xl overflow-hidden bg-gray-100 shadow-lg">
              <img
                src={book?.imageUrl || "/placeholder.svg?height=600&width=400&query=book cover"}
                alt={book?.title || "Book cover"}
                className="w-full h-full object-cover"
              />
            </div>

            {/* Badges */}
            <div className="absolute top-4 left-4 flex flex-col gap-2">
              {book?.bestseller && (
                <span className="bg-yellow-500 text-white text-xs font-bold px-3 py-1.5 rounded-full flex items-center">
                  <Award size={14} className="mr-1" />
                  Bestseller
                </span>
              )}
              {book?.newRelease && (
                <span className="bg-purple-600 text-white text-xs font-bold px-3 py-1.5 rounded-full flex items-center">
                  <Clock size={14} className="mr-1" />
                  New Release
                </span>
              )}
            </div>

            {/* Share button */}
            <button className="absolute top-4 right-4 bg-white/80 backdrop-blur-sm p-2 rounded-full shadow-md hover:bg-white transition-colors">
              <Share2 size={18} className="text-gray-700" />
            </button>
          </div>

          {/* Book details */}
          <div className="lg:w-2/3">
            <div className="flex flex-col h-full">
              <div className="flex-grow">
                <div className="flex flex-wrap items-start justify-between gap-4 mb-4">
                  <div>
                    <h1 className="text-3xl font-bold text-gray-900 mb-2">{book?.title || "Book Title"}</h1>
                    <p className="text-xl text-gray-600">
                      by{" "}
                      <span className="hover:text-purple-600 cursor-pointer transition-colors">
                        {book?.author || "Author Name"}
                      </span>
                    </p>
                  </div>

                  <div className="flex items-center gap-3">
                    <span className="text-2xl font-bold text-purple-600">${book?.price?.toFixed(2) || "0.00"}</span>
                    <span
                      className={`text-sm px-3 py-1 rounded-full font-medium ${
                        book?.availability === "In stock" ? "bg-green-100 text-green-800" : "bg-red-100 text-red-800"
                      }`}
                    >
                      {book?.availability || "Availability unknown"}
                    </span>
                  </div>
                </div>

                <div className="flex items-center mb-6">
                  <div className="flex">
                    {renderStars(book?.rating || 0)}
                    {hasHalfStar && (
                      <div className="relative">
                        <Star size={18} className="text-gray-300" />
                        <div className="absolute inset-0 overflow-hidden w-1/2">
                          <Star size={18} className="text-yellow-400 fill-yellow-400" />
                        </div>
                      </div>
                    )}
                  </div>
                  <span className="ml-2 text-gray-600">({book?.rating || 0} out of 5)</span>
                  <span className="mx-2 text-gray-400">•</span>
                  <a href="#reviews" className="text-purple-600 hover:text-purple-800 transition-colors">
                    Read reviews
                  </a>
                </div>

                <div className="mb-8">
                  <h3 className="text-lg font-semibold mb-3 text-gray-900">Description</h3>
                  <p className="text-gray-700 leading-relaxed">
                    {book?.description ||
                      "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed euismod, nisl vel ultricies lacinia, nisl nisl aliquam nisl, eget aliquam nisl nisl sit amet nisl. Sed euismod, nisl vel ultricies lacinia, nisl nisl aliquam nisl, eget aliquam nisl nisl sit amet nisl."}
                  </p>
                </div>
              </div>

              {/* Action buttons */}
              <div className="flex flex-wrap gap-3 mb-6">
                <button className="flex items-center gap-2 bg-purple-600 text-white px-6 py-3 rounded-lg hover:bg-purple-700 transition-colors shadow-sm font-medium">
                  <ShoppingCart size={18} />
                  <span>Add to Cart</span>
                </button>
                <button className="flex items-center gap-2 bg-gray-100 text-gray-800 px-6 py-3 rounded-lg hover:bg-gray-200 transition-colors font-medium">
                  <Bookmark size={18} />
                  <span>Bookmark</span>
                </button>
                <button className="flex items-center gap-2 bg-red-50 text-red-600 px-6 py-3 rounded-lg hover:bg-red-100 transition-colors font-medium">
                  <Heart size={18} />
                  <span>Add to Wishlist</span>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Book details section */}
      <div className="border-t border-gray-100 bg-gray-50 p-8 rounded-b-xl">
        <h2 className="text-xl font-semibold mb-6 text-gray-900">Book Details</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-y-4 gap-x-8">
          <div className="space-y-3">
            <p className="flex justify-between">
              <span className="text-gray-600 font-medium">Genre:</span>
              <span className="text-gray-900">{book?.genre || "Fiction"}</span>
            </p>
            <p className="flex justify-between">
              <span className="text-gray-600 font-medium">Format:</span>
              <span className="text-gray-900">Hardcover</span>
            </p>
            <p className="flex justify-between">
              <span className="text-gray-600 font-medium">Pages:</span>
              <span className="text-gray-900">342</span>
            </p>
            <p className="flex justify-between">
              <span className="text-gray-600 font-medium">Language:</span>
              <span className="text-gray-900">English</span>
            </p>
          </div>
          <div className="space-y-3">
            <p className="flex justify-between">
              <span className="text-gray-600 font-medium">Publisher:</span>
              <span className="text-gray-900">BookPress Publishing</span>
            </p>
            <p className="flex justify-between">
              <span className="text-gray-600 font-medium">Publication Date:</span>
              <span className="text-gray-900">June 15, 2023</span>
            </p>
            <p className="flex justify-between">
              <span className="text-gray-600 font-medium">ISBN:</span>
              <span className="text-gray-900">978-1234567890</span>
            </p>
            <p className="flex justify-between">
              <span className="text-gray-600 font-medium">Dimensions:</span>
              <span className="text-gray-900">6 × 9 × 1 inches</span>
            </p>
          </div>
        </div>
      </div>

      {/* Recommendations section */}
      <div className="border-t border-gray-100 p-8">
        <h2 className="text-xl font-semibold mb-6 text-gray-900">You might also like</h2>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          {[1, 2, 3, 4].map((item) => (
            <div key={item} className="group cursor-pointer">
              <div className="aspect-[3/4] rounded-lg overflow-hidden bg-gray-100 mb-2 group-hover:shadow-md transition-shadow">
                <img
                  src={`/abstract-book-cover.png?height=300&width=200&query=book cover ${item}`}
                  alt="Book recommendation"
                  className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
                />
              </div>
              <h3 className="font-medium text-gray-900 group-hover:text-purple-600 transition-colors">
                Recommended Book {item}
              </h3>
              <p className="text-sm text-gray-600">Author Name</p>
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}
