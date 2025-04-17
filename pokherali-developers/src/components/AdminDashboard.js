
import { useState } from "react"
import { PlusCircle, BookOpen, ShoppingBag, BarChart3, Search, Filter, ChevronDown, Download } from "lucide-react"
import AddBookModal from "./AddBook"

export default function AdminDashboard() {
  const [activeTab, setActiveTab] = useState("inventory")
  const [isAddBookModalOpen, setIsAddBookModalOpen] = useState(false)

  // Sample inventory data
  const [inventory, setInventory] = useState([
    { id: 1, title: "The Great Adventure", stock: 45, price: 24.99 },
    { id: 2, title: "Mystery of the Blue Lake", stock: 32, price: 19.99 },
    { id: 3, title: "The Science of Everything", stock: 18, price: 29.99 },
    { id: 4, title: "Cooking Masterclass", stock: 0, price: 34.99 },
    { id: 5, title: "Fantasy World", stock: 27, price: 22.99 },
  ])

  // Sample orders data
  const orders = [
    { id: "1234", customer: "John Doe", date: "2023-04-15", total: 45.98, status: "Delivered" },
    { id: "1235", customer: "Jane Smith", date: "2023-04-14", total: 29.99, status: "Processing" },
    { id: "1236", customer: "Robert Johnson", date: "2023-04-13", total: 67.5, status: "Shipped" },
    { id: "1237", customer: "Emily Davis", date: "2023-04-12", total: 22.99, status: "Delivered" },
  ]

  const handleAddBook = (newBook) => {
    // Add the new book to inventory with a new ID
    const newId = Math.max(...inventory.map((item) => item.id)) + 1
    setInventory([...inventory, { id: newId, ...newBook }])
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 py-8 sm:px-6 lg:px-8">
        <div className="flex flex-col md:flex-row md:items-center md:justify-between mb-8 gap-4">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Admin Dashboard</h1>
            <p className="mt-1 text-gray-600">Manage your bookstore inventory and orders</p>
          </div>
          <button
            onClick={() => setIsAddBookModalOpen(true)}
            className="flex items-center justify-center gap-2 bg-purple-600 text-white px-4 py-2.5 rounded-lg hover:bg-purple-700 transition-colors shadow-sm font-medium"
          >
            <PlusCircle size={18} />
            <span>Add New Book</span>
          </button>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
          <div className="bg-white p-6 rounded-xl border border-gray-200 shadow-sm hover:shadow-md transition-shadow">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-500">Total Books</p>
                <h2 className="text-3xl font-bold text-gray-900 mt-1">{inventory.length}</h2>
              </div>
              <div className="p-3 bg-purple-100 rounded-full">
                <BookOpen size={24} className="text-purple-600" />
              </div>
            </div>
            <div className="mt-4 text-sm text-green-600 font-medium">
              <span>↑ 12% from last month</span>
            </div>
          </div>

          <div className="bg-white p-6 rounded-xl border border-gray-200 shadow-sm hover:shadow-md transition-shadow">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-500">Low Stock</p>
                <h2 className="text-3xl font-bold text-gray-900 mt-1">
                  {inventory.filter((item) => item.stock < 20).length}
                </h2>
              </div>
              <div className="p-3 bg-amber-100 rounded-full">
                <Filter size={24} className="text-amber-600" />
              </div>
            </div>
            <div className="mt-4 text-sm text-red-600 font-medium">
              <span>↑ 3 items need restock</span>
            </div>
          </div>

          <div className="bg-white p-6 rounded-xl border border-gray-200 shadow-sm hover:shadow-md transition-shadow">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-500">Recent Orders</p>
                <h2 className="text-3xl font-bold text-gray-900 mt-1">{orders.length}</h2>
              </div>
              <div className="p-3 bg-teal-100 rounded-full">
                <ShoppingBag size={24} className="text-teal-600" />
              </div>
            </div>
            <div className="mt-4 text-sm text-green-600 font-medium">
              <span>↑ 8% from last week</span>
            </div>
          </div>
        </div>

        {/* Custom tabs implementation */}
        <div className="mb-6">
          <div className="flex flex-wrap border-b border-gray-200">
            <button
              onClick={() => setActiveTab("inventory")}
              className={`flex items-center gap-2 py-3 px-6 font-medium text-sm transition-colors ${
                activeTab === "inventory"
                  ? "border-b-2 border-purple-600 text-purple-600"
                  : "text-gray-500 hover:text-gray-700 hover:border-b-2 hover:border-gray-300"
              }`}
            >
              <BookOpen size={18} />
              <span>Inventory</span>
            </button>
            <button
              onClick={() => setActiveTab("orders")}
              className={`flex items-center gap-2 py-3 px-6 font-medium text-sm transition-colors ${
                activeTab === "orders"
                  ? "border-b-2 border-purple-600 text-purple-600"
                  : "text-gray-500 hover:text-gray-700 hover:border-b-2 hover:border-gray-300"
              }`}
            >
              <ShoppingBag size={18} />
              <span>Orders</span>
            </button>
            <button
              onClick={() => setActiveTab("analytics")}
              className={`flex items-center gap-2 py-3 px-6 font-medium text-sm transition-colors ${
                activeTab === "analytics"
                  ? "border-b-2 border-purple-600 text-purple-600"
                  : "text-gray-500 hover:text-gray-700 hover:border-b-2 hover:border-gray-300"
              }`}
            >
              <BarChart3 size={18} />
              <span>Analytics</span>
            </button>
          </div>
        </div>

        {/* Tab content container */}
        <div className="bg-white rounded-xl border border-gray-200 shadow-sm overflow-hidden">
          {/* Inventory Tab */}
          {activeTab === "inventory" && (
            <>
              <div className="p-6 border-b border-gray-200">
                <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
                  <div>
                    <h2 className="text-xl font-semibold text-gray-900">Book Inventory</h2>
                    <p className="text-sm text-gray-500 mt-1">Manage your book inventory</p>
                  </div>
                  <div className="flex flex-col sm:flex-row gap-3">
                    <div className="relative">
                      <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                        <Search size={16} className="text-gray-400" />
                      </div>
                      <input
                        type="text"
                        placeholder="Search books..."
                        className="pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-purple-500 w-full sm:w-auto"
                      />
                    </div>
                    <button className="flex items-center justify-center gap-2 bg-gray-100 text-gray-700 px-4 py-2 rounded-lg hover:bg-gray-200 transition-colors">
                      <Download size={16} />
                      <span>Export</span>
                    </button>
                  </div>
                </div>
              </div>
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead>
                    <tr className="bg-gray-50">
                      <th className="text-left py-3.5 px-6 text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Title
                      </th>
                      <th className="text-left py-3.5 px-6 text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Stock
                      </th>
                      <th className="text-left py-3.5 px-6 text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Price
                      </th>
                      <th className="text-left py-3.5 px-6 text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Status
                      </th>
                      <th className="text-left py-3.5 px-6 text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Actions
                      </th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-gray-200">
                    {inventory.map((item) => (
                      <tr key={item.id} className="hover:bg-gray-50 transition-colors">
                        <td className="py-4 px-6 text-sm font-medium text-gray-900">{item.title}</td>
                        <td className="py-4 px-6 text-sm text-gray-500">{item.stock}</td>
                        <td className="py-4 px-6 text-sm text-gray-500">${item.price.toFixed(2)}</td>
                        <td className="py-4 px-6">
                          <span
                            className={`px-2.5 py-1 rounded-full text-xs font-medium ${
                              item.stock === 0
                                ? "bg-red-100 text-red-800"
                                : item.stock < 20
                                  ? "bg-amber-100 text-amber-800"
                                  : "bg-green-100 text-green-800"
                            }`}
                          >
                            {item.stock === 0 ? "Out of Stock" : item.stock < 20 ? "Low Stock" : "In Stock"}
                          </span>
                        </td>
                        <td className="py-4 px-6">
                          <div className="flex gap-3">
                            <button className="text-sm font-medium text-purple-600 hover:text-purple-800 transition-colors">
                              Edit
                            </button>
                            <button className="text-sm font-medium text-red-600 hover:text-red-800 transition-colors">
                              Delete
                            </button>
                          </div>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
              <div className="p-4 border-t border-gray-200 bg-gray-50">
                <div className="flex items-center justify-between">
                  <p className="text-sm text-gray-500">
                    Showing {inventory.length} of {inventory.length} books
                  </p>
                  <div className="flex gap-2">
                    <button className="px-3 py-1 border border-gray-300 rounded-md text-sm bg-white text-gray-500 hover:bg-gray-50">
                      Previous
                    </button>
                    <button className="px-3 py-1 border border-gray-300 rounded-md text-sm bg-white text-gray-500 hover:bg-gray-50">
                      Next
                    </button>
                  </div>
                </div>
              </div>
            </>
          )}

          {/* Orders Tab */}
          {activeTab === "orders" && (
            <>
              <div className="p-6 border-b border-gray-200">
                <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
                  <div>
                    <h2 className="text-xl font-semibold text-gray-900">Recent Orders</h2>
                    <p className="text-sm text-gray-500 mt-1">Manage customer orders</p>
                  </div>
                  <div className="flex flex-col sm:flex-row gap-3">
                    <div className="relative">
                      <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                        <Search size={16} className="text-gray-400" />
                      </div>
                      <input
                        type="text"
                        placeholder="Search orders..."
                        className="pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-purple-500 w-full sm:w-auto"
                      />
                    </div>
                    <button className="flex items-center justify-center gap-2 bg-gray-100 text-gray-700 px-4 py-2 rounded-lg hover:bg-gray-200 transition-colors">
                      <Filter size={16} />
                      <span>Filter</span>
                      <ChevronDown size={16} />
                    </button>
                  </div>
                </div>
              </div>
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead>
                    <tr className="bg-gray-50">
                      <th className="text-left py-3.5 px-6 text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Order ID
                      </th>
                      <th className="text-left py-3.5 px-6 text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Customer
                      </th>
                      <th className="text-left py-3.5 px-6 text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Date
                      </th>
                      <th className="text-left py-3.5 px-6 text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Total
                      </th>
                      <th className="text-left py-3.5 px-6 text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Status
                      </th>
                      <th className="text-left py-3.5 px-6 text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Actions
                      </th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-gray-200">
                    {orders.map((order) => (
                      <tr key={order.id} className="hover:bg-gray-50 transition-colors">
                        <td className="py-4 px-6 text-sm font-medium text-gray-900">#{order.id}</td>
                        <td className="py-4 px-6 text-sm text-gray-500">{order.customer}</td>
                        <td className="py-4 px-6 text-sm text-gray-500">{order.date}</td>
                        <td className="py-4 px-6 text-sm font-medium text-gray-900">${order.total.toFixed(2)}</td>
                        <td className="py-4 px-6">
                          <span
                            className={`px-2.5 py-1 rounded-full text-xs font-medium ${
                              order.status === "Delivered"
                                ? "bg-green-100 text-green-800"
                                : order.status === "Shipped"
                                  ? "bg-blue-100 text-blue-800"
                                  : "bg-amber-100 text-amber-800"
                            }`}
                          >
                            {order.status}
                          </span>
                        </td>
                        <td className="py-4 px-6">
                          <button className="text-sm font-medium text-purple-600 hover:text-purple-800 transition-colors">
                            View Details
                          </button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
              <div className="p-4 border-t border-gray-200 bg-gray-50">
                <div className="flex items-center justify-between">
                  <p className="text-sm text-gray-500">
                    Showing {orders.length} of {orders.length} orders
                  </p>
                  <div className="flex gap-2">
                    <button className="px-3 py-1 border border-gray-300 rounded-md text-sm bg-white text-gray-500 hover:bg-gray-50">
                      Previous
                    </button>
                    <button className="px-3 py-1 border border-gray-300 rounded-md text-sm bg-white text-gray-500 hover:bg-gray-50">
                      Next
                    </button>
                  </div>
                </div>
              </div>
            </>
          )}

          {/* Analytics Tab */}
          {activeTab === "analytics" && (
            <>
              <div className="p-6 border-b border-gray-200">
                <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
                  <div>
                    <h2 className="text-xl font-semibold text-gray-900">Sales Analytics</h2>
                    <p className="text-sm text-gray-500 mt-1">View your store performance</p>
                  </div>
                  <div className="flex gap-3">
                    <select className="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-purple-500">
                      <option>Last 7 days</option>
                      <option>Last 30 days</option>
                      <option>Last 90 days</option>
                      <option>This year</option>
                    </select>
                    <button className="flex items-center justify-center gap-2 bg-gray-100 text-gray-700 px-4 py-2 rounded-lg hover:bg-gray-200 transition-colors">
                      <Download size={16} />
                      <span>Export</span>
                    </button>
                  </div>
                </div>
              </div>
              <div className="p-6">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
                  <div className="bg-gray-50 p-4 rounded-lg border border-gray-200">
                    <h3 className="text-sm font-medium text-gray-500 mb-2">Revenue</h3>
                    <p className="text-2xl font-bold text-gray-900">$1,245.32</p>
                    <p className="text-sm text-green-600 font-medium mt-1">↑ 12.5% from last month</p>
                  </div>
                  <div className="bg-gray-50 p-4 rounded-lg border border-gray-200">
                    <h3 className="text-sm font-medium text-gray-500 mb-2">Average Order Value</h3>
                    <p className="text-2xl font-bold text-gray-900">$42.65</p>
                    <p className="text-sm text-green-600 font-medium mt-1">↑ 3.2% from last month</p>
                  </div>
                </div>
                <div className="h-80 flex items-center justify-center bg-gray-50 rounded-lg border border-gray-200">
                  <div className="text-center">
                    <BarChart3 size={48} className="mx-auto text-gray-400 mb-2" />
                    <p className="text-gray-500">Sales chart will be displayed here</p>
                    <button className="mt-4 text-sm text-purple-600 font-medium hover:text-purple-800">
                      Generate Report
                    </button>
                  </div>
                </div>
              </div>
            </>
          )}
        </div>
      </div>

      {/* Add Book Modal */}
      <AddBookModal isOpen={isAddBookModalOpen} onClose={() => setIsAddBookModalOpen(false)} onSave={handleAddBook} />
    </div>
  )
}
