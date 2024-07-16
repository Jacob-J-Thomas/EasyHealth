import { ReadMe } from "./components/ReadMe";
import { ChatInterface } from "./components/ChatInterface";

const AppRoutes = [
  {
    index: true,
    element: <ChatInterface />
  },
  {
    path: '/fetch-data',
    element: <ReadMe />
  }
];

export default AppRoutes;
